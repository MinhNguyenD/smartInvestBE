
using Confluent.Kafka;
using notification.Dto;
using Newtonsoft.Json;

namespace notification.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly string topic;
        private readonly IConsumer<string, string> kafkaConsumer;
        private readonly EmailService _emailService;

        public KafkaConsumerService(IConfiguration configuration, EmailService emailService)
        {
            var kafkaSettings = configuration.GetSection("Kafka");
            topic = kafkaSettings.GetValue<string>("Topic");
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = kafkaSettings.GetValue<string>("BootstrapServers"),
                GroupId = kafkaSettings.GetValue<string>("GroupId"),
            };
            this.kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            _emailService = emailService;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            kafkaConsumer.Subscribe(this.topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumerResult = this.kafkaConsumer.Consume(cancellationToken);
                    Console.WriteLine($"{consumerResult.Message.Key}: {consumerResult.Message.Value}ms");
                    EmailMessage emailMessage = JsonConvert.DeserializeObject<EmailMessage>(consumerResult.Message.Value);
                    _emailService.SendEmail(emailMessage);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    Console.WriteLine($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception e)
                {
                    // Handle failure: retry, log, or send to dead-letter topic
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }

        public override void Dispose()
        {
            this.kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
            this.kafkaConsumer.Dispose();
            base.Dispose();
        }
    }
}

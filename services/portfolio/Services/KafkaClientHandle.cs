using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace api.Services
{
    public class KafkaClientHandle : IDisposable
    {
        IProducer<byte[], byte[]> _kafkaProducer;
        IAdminClient _kafkaAdminClient;
        public KafkaClientHandle(IConfiguration configuration)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers"),
            };
            _kafkaProducer = new ProducerBuilder<byte[], byte[]>(producerConfig).Build();
            var adminClientConfig = new AdminClientConfig { BootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers") };
            _kafkaAdminClient = new AdminClientBuilder(adminClientConfig).Build(); 
        }

        public Handle Handle { get => this._kafkaProducer.Handle; }
        public void Dispose()
        {
            _kafkaProducer.Flush();
            _kafkaProducer.Dispose();
            _kafkaAdminClient.Dispose();
        }

        public async Task CreateTopicAsync(string topicName, int numPartitions = 1, short replicationFactor = 1)
        {
            try
            {
                // Define topic specifications
                var topicSpec = new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor
                };

                // Create the topic
                await _kafkaAdminClient.CreateTopicsAsync(new TopicSpecification[] { topicSpec });
                Console.WriteLine($"Topic '{topicName}' created successfully.");
            }
            catch (CreateTopicsException e)
            {
                if (e.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
                {
                    Console.WriteLine($"Topic '{topicName}' already exists.");
                }
                else
                {
                    Console.WriteLine($"An error occurred creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                }
            }
        }
    }
}

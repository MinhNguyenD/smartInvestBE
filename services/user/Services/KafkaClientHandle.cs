using Confluent.Kafka;

namespace api.Services
{
    public class KafkaClientHandle : IDisposable
    {
        IProducer<byte[], byte[]> kafkaProducer;
        public KafkaClientHandle(IConfiguration configuration)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers"),
            };
            kafkaProducer = new ProducerBuilder<byte[], byte[]>(producerConfig).Build();
        }
        public Handle Handle { get => this.kafkaProducer.Handle; }
        public void Dispose()
        {
            kafkaProducer.Flush();
            kafkaProducer.Dispose();
        }
    }
}

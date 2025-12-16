using Confluent.Kafka;
using System.Collections.Concurrent;

namespace api.Services
{
    public class KafkaProducerService<K, V>
    {
        private readonly IProducer<K, V> kafkaHandle;
        public KafkaProducerService(KafkaClientHandle handle)
        {
            kafkaHandle = new DependentProducerBuilder<K, V>(handle.Handle).Build();
        }

        public Task ProduceAsync(string topic, Message<K, V> message)
            => this.kafkaHandle.ProduceAsync(topic, message);

        public void Produce(string topic, Message<K, V> message, Action<DeliveryReport<K, V>> deliveryHandler = null)
           => this.kafkaHandle.Produce(topic, message, deliveryHandler);

        public void Flush(TimeSpan timeout)
            => this.kafkaHandle.Flush(timeout);
    }
}

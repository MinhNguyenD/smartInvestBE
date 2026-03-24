using Confluent.Kafka;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
namespace api.Services
{
    public class KafkaProducerService<K, V>
    {
        private readonly IProducer<K, V> kafkaHandle;
        static readonly ActivitySource ProducerActivity = new("KafkaProducer");
        static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        public KafkaProducerService(KafkaClientHandle handle)
        {
            kafkaHandle = new DependentProducerBuilder<K, V>(handle.Handle).Build();
        }

        public Task ProduceAsync(string topic, Message<K, V> message)
        {
            using var activity = ProducerActivity.StartActivity("kafka.produce", ActivityKind.Producer);
            if (activity != null)
            {
                var headers = new Headers();

                // Inject trace context into Kafka headers
                Propagator.Inject(
                    new PropagationContext(activity.Context, Baggage.Current),
                    headers,
                    (h, k, v) => h.Add(k, System.Text.Encoding.UTF8.GetBytes(v)));
                message.Headers = headers;
            }
            return this.kafkaHandle.ProduceAsync(topic, message);
        }

        public void Produce(string topic, Message<K, V> message, Action<DeliveryReport<K, V>> deliveryHandler = null)
           => this.kafkaHandle.Produce(topic, message, deliveryHandler);

        public void Flush(TimeSpan timeout)
            => this.kafkaHandle.Flush(timeout);
    }
}

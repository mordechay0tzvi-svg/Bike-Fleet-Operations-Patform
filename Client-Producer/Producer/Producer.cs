using System.Text.Json;
using Confluent.Kafka;
namespace Producer;
public class KafkaProducer
{
    private readonly  IProducer<string, string> _producer;
    public KafkaProducer(string bootstrapServers)
    {
        var config = new ProducerConfig{BootstrapServers = bootstrapServers};
        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    public async Task<DeliveryResult<string,string>> SendAsync<T>(string key, string topicname, T send)
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = JsonSerializer.Serialize(send)
        };
        return await _producer.ProduceAsync(topicname, message);
    }
    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}
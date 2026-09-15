using System.Security.AccessControl;
using System.Text.Json;
using Confluent.Kafka;
namespace Consumer;
public class KafkaConsumer
{
    private readonly IConsumer<string, string> _consumer;
   public KafkaConsumer(string bootstrapserver, string groupId)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapserver,
            GroupId = groupId + Guid.NewGuid(),
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public void Subscribe(string topic)
    {
        System.Console.WriteLine("attempting subscribe");
        _consumer.Subscribe(topic);
        System.Console.WriteLine("subscribed");
    }
    public T? Consume<T>()
    {
        var message = _consumer.Consume().Message.Value;
        if (message != null)
        {
            return JsonSerializer.Deserialize<T>(message);
        }
        return default;
    }
    public void Commit()
    {
        _consumer.Commit();
    }
}
using Models;
using Consumer;
using Redis;
using DataBase;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
namespace Handlers;
public interface IStatusHandler
{
    Task HandleAsync(string topic);
}
public class StatusHandler : IStatusHandler
{
    private readonly KafkaConsumer _consumer;
    private readonly MongoDbService _mongo;
    private readonly RedisService _redis;
    private readonly ILogger<StatusHandler> _logger;
    public StatusHandler(KafkaConsumer consumer,MongoDbService mongo, RedisService redis, ILogger<StatusHandler> logger)
    {
        _consumer = consumer;
        _mongo = mongo;
        _redis = redis;
        _logger = logger;
    }
    public async Task HandleAsync(string topic)
    {
        _consumer.Subscribe(topic);
        while (true)
        {
            var status = _consumer.Consume<StationStatus>();
            if (status == null)
            {
                _logger.LogWarning("Couldn't consume station status");
                continue;
            }
            var key = $"station-status:{status.StationId}";
            var previous = await _redis.GetAsync<StationStatus>(key);
            if (previous != null && !StatusChecker.IsStatusChanged(previous, status))
            {
                _logger.LogInformation("No change for station {StationId}",status.StationId);
                continue;
            }
            var found = await _mongo.StationStatus.Find(s => s.StationId == status.StationId).FirstOrDefaultAsync();
            if (found != null)
            {
                await _mongo.StationStatus.ReplaceOneAsync(s => s.StationId == status.StationId,status);
            }
            else
            {
                await _mongo.StationStatus.InsertOneAsync(status);
            }
            await _redis.SetAsync(key, status);
        }
    }
}
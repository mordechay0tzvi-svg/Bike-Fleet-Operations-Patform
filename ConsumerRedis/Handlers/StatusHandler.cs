using Models;
using Consumer;
using Redis;
using DataBase;
using Microsoft.Extensions.Logging;
namespace Handlers;
public interface IStatusHandler
{
    Task HandleAsync(string topic);
}
public class StatusHandler : IStatusHandler
{
    private readonly KafkaConsumer _consumer;
    private readonly DbAppContext _context;
    private readonly RedisService _redis;
    private readonly ILogger<StatusHandler> _logger;
    public StatusHandler(KafkaConsumer consumer, DbAppContext context,RedisService redis, ILogger<StatusHandler> logger)
    {
        _consumer = consumer;
        _context = context;
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
            var found = await _context.StationStatus.FindAsync(status.StationId);
            if (found != null)
            {
                _context.Entry(found).CurrentValues.SetValues(status);
            }
            else
            {
                await _context.StationStatus.AddAsync(status);
            }
            await _context.SaveChangesAsync();
            await _redis.SetAsync(key, status);
        }
    }
}
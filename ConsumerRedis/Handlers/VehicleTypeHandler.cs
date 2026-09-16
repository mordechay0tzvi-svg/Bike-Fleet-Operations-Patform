using Models;
using Consumer;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using DataBase;
namespace Handlers;
public interface IVehicleTypeHandler
{
    Task HandleAsync(string topic);
}
public class VehicleTypeHandler : IVehicleTypeHandler
{
    private readonly KafkaConsumer _consumer;
    private readonly ILogger<VehicleTypeHandler> _logger;
    private readonly DbAppContext _context;

    public VehicleTypeHandler(KafkaConsumer consumer, DbAppContext context,ILogger<VehicleTypeHandler> logger)
    {
        _consumer = consumer;
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(string topic)
    {
        _consumer.Subscribe(topic);
        while (true)
        {
            var vehicleType = _consumer.Consume<VehicleType>();
            if (vehicleType == null)
            {
                _logger.LogWarning("Couldn't consume vehicle type");
                continue;
            }
             await _context.VehicleTypes.AddAsync(vehicleType);
             await _context.SaveChangesAsync();
        }
    }
}


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
    private readonly MongoDbService _mongo;
    private readonly ILogger<VehicleTypeHandler> _logger;

    public VehicleTypeHandler(KafkaConsumer consumer,MongoDbService mongo,ILogger<VehicleTypeHandler> logger)
    {
        _consumer = consumer;
        _mongo = mongo;
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
            var filter = Builders<VehicleType>.Filter.Eq(x => x.VehicleTypeId, vehicleType.VehicleTypeId);
            var options = new ReplaceOptions
            {
                IsUpsert = true
            };
            await _mongo.VehicleTypes.ReplaceOneAsync(filter,vehicleType,options);
        }
    }
}

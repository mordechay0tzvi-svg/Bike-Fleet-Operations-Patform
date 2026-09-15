using MongoDB.Driver;
using Models;
namespace DataBase;
public class MongoDbService
{
    private readonly IMongoDatabase _database;
    public MongoDbService(string ConnectionString)
    {
        var client = new MongoClient(ConnectionString);
        _database = client.GetDatabase("bikeStationDb");
    }
    public IMongoCollection<VehicleType> VehicleTypes => _database.GetCollection<VehicleType>("vehicleTypes");
}
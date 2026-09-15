using Consumer;
using DataBase;
using Handlers;
using Redis;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

var connectionString ="Server=localhost;Port=3306;Database=bikeStationDb;User=root;Password=secret;";

builder.Services.AddDbContext<DbAppContext>(options =>options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton<RedisService>(sp =>new RedisService("localhost:6379"));

builder.Services.AddSingleton<MongoDbService>(sp => new MongoDbService("mongodb://localhost:27017"));

builder.Services.AddSingleton<KafkaConsumer>(sp => new KafkaConsumer("localhost:9092"));

builder.Services.AddSingleton<IInformationHandler, InformationHandler>();
builder.Services.AddSingleton<IStatusHandler, StatusHandler>();
builder.Services.AddSingleton<IVehicleTypeHandler, VehicleTypeHandler>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DbAppContext>();
    context.Database.EnsureDeleted();
    await context.Database.EnsureCreatedAsync();
    var mongo = scope.ServiceProvider.GetRequiredService<MongoDbService>();
    mongo.VehicleTypes.Database.CreateCollection("bikeStationDb");
}

var informationHandler = host.Services.GetRequiredService<IInformationHandler>();
var statusHandler = host.Services.GetRequiredService<IStatusHandler>();
var vehicleTypeHandler = host.Services.GetRequiredService<IVehicleTypeHandler>();

await Task.WhenAll(
    informationHandler.HandleAsync("bike.station-information"),
    statusHandler.HandleAsync("bike.station-status"),
    vehicleTypeHandler.HandleAsync("bike.vehicle-types")
);

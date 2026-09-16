using Redis;
using Consumer;
using DataBase;
using Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = "Server=localhost;Port=3306;Database=bikeStationDb;User=root;Password=secret;";

builder.Services.AddDbContext<DbAppContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton<RedisService>(sp => new RedisService("localhost:6379"));

builder.Services.AddSingleton<MongoDbService>(sp => new MongoDbService("mongodb://localhost:27017"));

builder.Services.AddScoped<IStatusHandler>(sp => new StatusHandler(
    new KafkaConsumer("localhost:9092", "statusId"),
    sp.GetRequiredService<MongoDbService>(),
    sp.GetRequiredService<RedisService>(),
    sp.GetRequiredService<ILogger<StatusHandler>>()));

builder.Services.AddScoped<IInformationHandler>(sp => new InformationHandler(
    new KafkaConsumer("localhost:9092", "informationId"),
    sp.GetRequiredService<DbAppContext>(),
    sp.GetRequiredService<ILogger<InformationHandler>>()));

builder.Services.AddScoped<IVehicleTypeHandler>(sp => new VehicleTypeHandler(
    new KafkaConsumer("localhost:9092", "vehicleTypeId"),
    sp.GetRequiredService<DbAppContext>(),
    sp.GetRequiredService<ILogger<VehicleTypeHandler>>()));

var host = builder.Build();

using (var creatingScope = host.Services.CreateScope())
{
    var context = creatingScope.ServiceProvider.GetRequiredService<DbAppContext>();
    var redis = creatingScope.ServiceProvider.GetRequiredService<RedisService>();
    await redis.ClearAsync();
    context.Database.EnsureDeleted();
    await context.Database.EnsureCreatedAsync();
}

using var scope = host.Services.CreateScope();

var statusHandler = scope.ServiceProvider.GetRequiredService<IStatusHandler>();
var informationHandler = scope.ServiceProvider.GetRequiredService<IInformationHandler>();
var vehicleTypeHandler = scope.ServiceProvider.GetRequiredService<IVehicleTypeHandler>();

await Task.WhenAll(
    Task.Run(async () =>{await statusHandler.HandleAsync("bike.station-status");}),
    Task.Run(async () =>{await informationHandler.HandleAsync("bike.station-information");}), 
    Task.Run(async () => {await vehicleTypeHandler.HandleAsync("bike.vehicle-types");})
);

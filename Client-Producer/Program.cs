using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer;
using Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(sp => new KafkaProducer("localhost:9092"));

builder.Services.AddHttpClient<IStationStatusService, StationStatusService>();
builder.Services.AddHttpClient<IStationInformationService, StationInformationService>();
builder.Services.AddHttpClient<IVehiclesService, VehicleTypesService>();

using var host = builder.Build();

var statusService = host.Services.GetRequiredService<IStationStatusService>();
var infoService = host.Services.GetRequiredService<IStationInformationService>();
var vehiclesService = host.Services.GetRequiredService<IVehiclesService>();

var statusTask = RunPeriodicAsync(TimeSpan.FromMinutes(1),
    () => statusService.FetchAndPublishAsync("https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_status.json"));
var infoTask = RunPeriodicAsync(TimeSpan.FromMinutes(60),
    () => infoService.FetchAndPublishAsync("https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_information.json"));
var vehiclesTask = RunPeriodicAsync(TimeSpan.FromMinutes(60),
    () => vehiclesService.FetchAndPublishAsync("https://gbfs.lyft.com/gbfs/2.3/bkn/en/vehicle_types.json"));

await Task.WhenAll(statusTask, infoTask, vehiclesTask);

static async Task RunPeriodicAsync(TimeSpan interval,Func<Task> action)
{
    using var timer = new PeriodicTimer(interval);
    await action();
    while (await timer.WaitForNextTickAsync())
    {
        await action();
    }
}

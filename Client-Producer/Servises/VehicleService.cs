using System.Net.Http.Json;
using Producer;
using Dto;
using Microsoft.Extensions.Logging;
namespace Services;
public interface IVehiclesService
{
    Task FetchAndPublishAsync(string url);
}
public class VehicleTypesService : IVehiclesService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VehicleTypesService> _logger;
    private readonly KafkaProducer _kafkaProducer;
    public VehicleTypesService(HttpClient httpClient, ILogger<VehicleTypesService> logger, KafkaProducer kafkaProducer)
    {
        _httpClient = httpClient;
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }
    public async Task FetchAndPublishAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<GbfsVehicleTypeResponse>(url);
            if (response?.Data?.VehicleTypes == null)
            {
                _logger.LogWarning("Received empty or invalid JSON payload from GBFS vehile-types endpoint.");
                return;
            }
            foreach (var vehicleType in response.Data.VehicleTypes)
            {
                if (String.IsNullOrWhiteSpace(vehicleType.VehicleTypeId))
                {
                    _logger.LogWarning("No Id found for this Vehicle-type. Skipping");
                    continue;
                }
                await _kafkaProducer.SendAsync<VehicleTypeDto>(vehicleType.VehicleTypeId, "bike.vehicle-types", vehicleType);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch or process GBFS vehicle type.");
        }
    }
}

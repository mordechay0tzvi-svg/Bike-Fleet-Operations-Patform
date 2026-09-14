using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Producer;
using Dto;
namespace Services;
public interface IStationStatusService
{
    Task FetchAndPublishAsync(string url);
}
public class StationStatusService : IStationStatusService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StationStatusService> _logger;
    private readonly KafkaProducer _kafkaProducer;
    public StationStatusService(HttpClient httpClient, ILogger<StationStatusService> logger, KafkaProducer kafkaProducer)
    {
        _httpClient = httpClient;
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }
    public async Task FetchAndPublishAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<GbfsStatusResponse>(url);
            if (response?.Data?.Stations == null)
            {
                _logger.LogWarning("Received empty or invalid JSON from GBFS status url.");
                return;
            }
            foreach (var station in response.Data.Stations)
            {
                if (!ValidateStationStatus(station))
                {
                    _logger.LogWarning("Station {StationId} failed basic validation. Skipping.", station.StationId);
                    continue;
                }
                await _kafkaProducer.SendAsync<StationStatusDto>(station.StationId, "bike.station-status", station);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch or process GBFS station status.");
        }
    }
    private bool ValidateStationStatus(StationStatusDto station)
    {
        if (string.IsNullOrWhiteSpace(station.StationId))
        {
            return false;
        }    
        if (station.NumVehiclesAvailable < 0 || station.NumDocksAvailable < 0)
        {
            return false;
        } 
        return true;
    }
}
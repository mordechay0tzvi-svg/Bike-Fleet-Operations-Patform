using System.Net.Http.Json;
using Producer;
using Dto;
using Microsoft.Extensions.Logging;
namespace Services;
public interface IStationInformationService
{
    Task FetchAndPublishAsync(string url);
}
public class StationInformationService : IStationInformationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StationInformationService> _logger;
    private readonly KafkaProducer _kafkaProducer;

    public StationInformationService(HttpClient httpClient, ILogger<StationInformationService> logger, KafkaProducer kafkaProducer)
    {
        _httpClient = httpClient;
        _logger = logger;
        _kafkaProducer = kafkaProducer;

    }
    public async Task FetchAndPublishAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<GbfsInformationResponse>(url);
            if (response?.Data?.Stations == null)
            {
                _logger.LogWarning("Received empty or invalid JSON payload from GBFS information endpoint.");
                return;
            }
            foreach (var station in response.Data.Stations)
            {
                if (!ValidateStationInformation(station))
                {
                    _logger.LogWarning("Station information for '{StationId}' failed validation. Skipping.", station.StationId);
                    continue;
                }
                await _kafkaProducer.SendAsync<StationInformationDto>(station.StationId, "bike.station-information", station);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch or process GBFS station information.");
        }
    }
    private bool ValidateStationInformation(StationInformationDto station)
    {
        if (string.IsNullOrWhiteSpace(station.StationId)) 
        {
            return false;
        }
        if (station.Lat < -90 || station.Lat > 90)
        {
            return false;
        }
        if (station.Lon < -180 || station.Lon > 180) 
        {
            return false;
        }
        if (station.Capacity < 0) 
        {
            return false;
        }
        return true;
    }
}
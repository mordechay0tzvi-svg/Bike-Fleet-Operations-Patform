using System.Text.Json.Serialization;
namespace Dto;
public class GbfsStatusResponse
{
    public GbfsStatusData Data { get; set; } = null!;
}
public class GbfsStatusData
{
    public List<StationStatusDto> Stations { get; set; } = [];
}

public class StationStatusDto
{
    [JsonPropertyName("station_id")]
    public string StationId { get; set; } = "";

    [JsonPropertyName("num_vehicles_available")]
    public int NumVehiclesAvailable { get; set; }

    [JsonPropertyName("num_docks_available")]
    public int NumDocksAvailable { get; set; }

    [JsonPropertyName("is_renting")]
    public int IsRenting { get; set; }

    [JsonPropertyName("is_returning")]
    public int IsReturning { get; set; }

    [JsonPropertyName("last_reported")]
    public long LastReported { get; set; }
}

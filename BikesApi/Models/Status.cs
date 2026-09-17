using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Models;
public class StationStatus
{
    [JsonPropertyName("station_id"), Key]
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
    // public StationInformation Station { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Models;
public class StationInformation
{
    [JsonPropertyName("station_id"), Key]
    public string StationId { get; set; } = "";
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("lat")]
    public double Lat { get; set; }
    [JsonPropertyName("lon")]
    public double Lon { get; set; }
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
    // public StationStatus? Status { get; set; }
}

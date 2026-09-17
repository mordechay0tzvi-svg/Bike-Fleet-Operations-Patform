using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Models;
public class VehicleType
{
    [JsonPropertyName("vehicle_type_id"), Key]
    public string VehicleTypeId { get; set; } = "";
    [JsonPropertyName("form_factor")]
    public string FormFactor { get; set; } = "";
    [JsonPropertyName("propulsion_type")]
    public string PropulsionType { get; set; } = "";
    [JsonPropertyName("max_range_meters")]
    public double? MaxRangeMeters { get; set; }
}

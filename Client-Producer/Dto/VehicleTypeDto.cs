using System.Text.Json.Serialization;
namespace Dto;
public class GbfsVehicleTypeResponse
{
    public GbfsVehicleTypeData Data { get; set; } = null!;
}
public class GbfsVehicleTypeData
{
    [JsonPropertyName("vehicle_types")]
    public List<VehicleTypeDto> VehicleTypes { get; set; } = [];
}
public class VehicleTypeDto
{
    [JsonPropertyName("vehicle_type_id")]
    public string VehicleTypeId { get; set; } = "";

    [JsonPropertyName("form_factor")]
    public string FormFactor { get; set; } = "";

    [JsonPropertyName("propulsion_type")]
    public string PropulsionType { get; set; } = "";

    [JsonPropertyName("max_range_meters")]
    public double? MaxRangeMeters { get; set; }
}

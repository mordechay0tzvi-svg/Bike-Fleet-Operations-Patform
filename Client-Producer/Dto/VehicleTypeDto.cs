using System.Text.Json.Serialization;
namespace Dto;
public record GbfsVehicleTypeResponse([property: JsonPropertyName("data")] GbfsVehicleTypeData Data);
public record GbfsVehicleTypeData([property: JsonPropertyName("Vehicles")] List<VehicleTypeDto> VehicleTypes);
public record VehicleTypeDto(
    [property: JsonPropertyName("vehicle_type_id")] string VehicleTypeId,
    [property: JsonPropertyName("form_factor")] string FormFactor,
    [property: JsonPropertyName("propulsion_type")] string PropulsionType,
    [property: JsonPropertyName("max_range_meters")] double? MaxRangeMeters
);
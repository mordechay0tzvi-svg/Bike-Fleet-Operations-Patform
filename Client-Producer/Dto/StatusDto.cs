using System.Text.Json.Serialization;
namespace Dto;
public record GbfsStatusResponse([property: JsonPropertyName("data")] GbfsStatusData Data);
public record GbfsStatusData([property: JsonPropertyName("stations")] List<StationStatusDto> Stations);

public record StationStatusDto(
    [property: JsonPropertyName("station_id")] string StationId,
    [property: JsonPropertyName("num_vehicles_available")] int NumVehiclesAvailable,
    [property: JsonPropertyName("num_docks_available")] int NumDocksAvailable,
    [property: JsonPropertyName("is_renting")] bool IsRenting,
    [property: JsonPropertyName("is_returning")] bool IsReturning,
    [property: JsonPropertyName("last_reported")] long LastReported
);
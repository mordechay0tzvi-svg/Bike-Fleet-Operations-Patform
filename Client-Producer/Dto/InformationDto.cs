using System.Text.Json.Serialization;
namespace Dto;
public record GbfsInformationResponse([property: JsonPropertyName("data")] GbfsInformationData Data);
public record GbfsInformationData([property: JsonPropertyName("stations")] List<StationInformationDto> Stations);
public record StationInformationDto(
    [property: JsonPropertyName("station_id")] string StationId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lon")] double Lon,
    [property: JsonPropertyName("capacity")] int Capacity
);
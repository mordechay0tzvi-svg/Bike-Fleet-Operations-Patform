using System.Text.Json.Serialization;
namespace Dto;
public class GbfsInformationResponse
{
    public GbfsInformationData Data { get; set; } = null!;
}
public class GbfsInformationData
{
    public List<StationInformationDto> Stations { get; set; } = [];
}
public class StationInformationDto
{
    [JsonPropertyName("station_id")]
    public string StationId { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
}

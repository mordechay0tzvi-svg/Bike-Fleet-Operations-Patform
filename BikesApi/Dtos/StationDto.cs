namespace Dtos;
public class StationDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Capacity { get; set; }
    public int AvailableBikes { get; set; }
    public int AvailableDocks { get; set; }
    public bool IsRenting { get; set; }
    public bool IsReturning { get; set; }
}

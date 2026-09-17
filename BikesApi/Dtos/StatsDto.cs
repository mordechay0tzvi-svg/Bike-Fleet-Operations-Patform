namespace Dtos;
public class StatsDto
{
    public int TotalStations { get; set; }
    public int EmptyStations { get; set; }
    public int FullStations { get; set; }
    public int LowAvailabilityStations { get; set; }
    public int OutOfServiceStations { get; set; }
    public DateTime LastUpdated { get; set; }
}

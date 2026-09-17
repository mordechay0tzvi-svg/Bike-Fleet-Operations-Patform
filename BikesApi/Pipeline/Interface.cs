using Dtos;
using Models;
namespace Pipeline;
public interface IPipeline
{
    Task<List<StationDto>?> GetStations(int? minAvBikes, bool? isRenting, bool? isReturning);
    Task<StationDto?> GetStationById(int id);
    Task<List<StatusHistoryDto>?> GetStationHistory(int id, DateTime? from, DateTime? to, int? limit);
    Task<StationStatus?> GetCurrentStationStatus(int id);
    Task<StatsDto> GetSystemDashboard();
}
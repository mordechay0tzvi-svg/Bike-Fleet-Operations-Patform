using DataBase;
using Models;
using Dtos;
using MongoDB.Driver;
using Redis;
using Microsoft.EntityFrameworkCore;
namespace Pipeline;
public class ApiPipeline : IPipeline
{
    private readonly DbAppContext _context;
    private readonly MongoDbService _mongo;
    private readonly RedisService _redis;
    public ApiPipeline(DbAppContext context, MongoDbService mongo, RedisService redis)
    {
        _context = context;
        _mongo = mongo;
        _redis = redis;
    }
    public async Task<List<StationDto>?> GetStations(int? minAvBikes, bool? isRenting, bool? isReturning)
    {
        IEnumerable<StationStatus> status = await _redis.GetByPatternAsync<StationStatus>("station-status*");
        if (minAvBikes.HasValue)
        {
            status = status.Where(x => x.NumDocksAvailable >= minAvBikes);
        }
        if (isRenting.HasValue)
        {
            status = status.Where(x => x.IsRenting == (isRenting.Value ? 1 : 0));
        }
        if (isReturning.HasValue)
        {
             status = status.Where(x => x.IsReturning == (isReturning.Value ? 1 : 0));
        }
        var ids = status.Select(s => s.StationId).ToList();
        var information = _context.StationInformation.Where(i => ids.Contains(i.StationId)).ToList();
        List<StationDto> filtered = new();
        foreach (var id in ids)
        {
            var st = status.FirstOrDefault(s => s.StationId == id)!; 
            var inf = information.FirstOrDefault(s => s.StationId == id)!;
            var dto = new StationDto
            {
                Id = st.StationId,
                Name = inf.Name,
                Latitude = inf.Lat,
                Longitude = inf.Lon,
                Capacity = inf.Capacity,
                AvailableBikes = st.NumVehiclesAvailable,
                AvailableDocks = st.NumDocksAvailable,
                IsRenting = st.IsRenting == 1,
                IsReturning = st.IsReturning == 1
            };
            filtered.Add(dto);
        }
        return filtered;
    }
    public async Task<StationDto?> GetStationById(int id)
    {
        IEnumerable<StationStatus> statuses = await _redis.GetByPatternAsync<StationStatus>("station-status:*");
        var status = statuses.FirstOrDefault(s => s.StationId == id.ToString());
        var information = _context.StationInformation.FirstOrDefault(i => i.StationId == id.ToString());
        if (status == null || information == null)
        {
            return null;
        }
        else
        {
            return new StationDto
            {
                Id = status.StationId,
                Name = information.Name,
                Latitude = information.Lat,
                Longitude = information.Lon,
                Capacity = information.Capacity,
                AvailableBikes = status.NumVehiclesAvailable,
                AvailableDocks = status.NumDocksAvailable,
                IsRenting = status.IsRenting == 1,
                IsReturning = status.IsReturning == 1
            };
        }
    }
    public async Task<StationStatus?> GetCurrentStationStatus(int id)
    {
        IEnumerable<StationStatus> statuses = await _redis.GetByPatternAsync<StationStatus>("station-status*");
        return statuses.FirstOrDefault(s => s.StationId == id.ToString());
    }
    public async Task<List<StatusHistoryDto>?> GetStationHistory(int id, DateTime? from, DateTime? to, int? limit)
    {
        var query = _mongo.StationStatus.AsQueryable();
        var stationIdStr = id.ToString();
        query = query.Where(s => s.StationId == stationIdStr);
        if (from.HasValue)
        {
            var fromTimestamp = new DateTimeOffset(from.Value).ToUnixTimeSeconds();
            query = query.Where(s => s.LastReported >= fromTimestamp);
        }
        if (to.HasValue)
        {
            var toTimestamp = new DateTimeOffset(to.Value).ToUnixTimeSeconds();
            query = query.Where(s => s.LastReported <= toTimestamp);
        }
        query = query.OrderByDescending(s => s.LastReported);
        if (limit.HasValue && limit.Value > 0)
        {
            query = query.Take(limit.Value);
        }
        return await  query.Select(s => 
        new StatusHistoryDto{
        Timestamp = DateTimeOffset.FromUnixTimeSeconds(s.LastReported).UtcDateTime,
        AvailableBikes = s.NumVehiclesAvailable,
        AvailableDocks = s.NumDocksAvailable}).ToListAsync();
    }
    public async Task<StatsDto> GetSystemDashboard()
    {
        var status = await _redis.GetByPatternAsync<StationStatus>("station-status*");
        
        return new StatsDto
        {
            TotalStations = _context.StationInformation.ToList().Count(),
            EmptyStations = status.Count(s => s.NumVehiclesAvailable == 0),
            FullStations = status.Count(s => s.NumDocksAvailable == 0),
            LowAvailabilityStations = status.Count(s => s.NumVehiclesAvailable <= 4),
            OutOfServiceStations = status.Count(s => s.IsRenting == 0 && s.IsReturning == 0),
            LastUpdated = DateTimeOffset.FromUnixTimeSeconds(status.Max(s => s.LastReported)).DateTime
        };
    }
}
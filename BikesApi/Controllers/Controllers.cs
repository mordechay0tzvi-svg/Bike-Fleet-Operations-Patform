using Models;
using Dtos;
using Pipeline;
using Microsoft.AspNetCore.Mvc;
using Redis;
namespace Controller;
[ApiController]
[Route("api/[controller]")]
public class StationsController : ControllerBase
{
    private readonly IPipeline _pipeline;
    private readonly RedisService _redis;
    public StationsController(IPipeline pipeline, RedisService redis)
    {
        _pipeline = pipeline;
        _redis = redis;
    }
    [HttpGet()]
    public async Task<ActionResult<List<StationDto>?>> GetStations([FromQuery]int? minAvBikes, [FromQuery]bool? isRenting, [FromQuery]bool? isReturning)
    {
        var response = await _pipeline.GetStations(minAvBikes, isRenting, isReturning);
        if (response == null)
        {
            return Ok(new List<StationDto>());
        }
        return Ok(response);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<StationDto?>> GetStationById([FromQuery] int id)
    {
        var response = await _pipeline.GetStationById(id);
        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }
    [HttpGet("{id}/status")]
    public async Task<ActionResult<StationStatus?>> GetCurrentStationStatus(int id)
    {
        var response = await _pipeline.GetCurrentStationStatus(id);
        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }
    [HttpGet("{id}/history")]
    public async Task<ActionResult<List<StatusHistoryDto>?>> GetStationHistory([FromQuery]int id, [FromQuery]DateTime? from, [FromQuery]DateTime? to, [FromQuery]int? limit)
    {
        var response = await _pipeline.GetStationHistory(id, from, to, limit);
        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }
    [HttpGet("dashbord")]
    public async Task<ActionResult<StatsDto>> GetSystemDashboard()
    {
        return Ok(await _pipeline.GetSystemDashboard());
    }
    [HttpGet("debug/redis")]
    public async Task<IActionResult> DebugRedis()
    {
        var result = await _redis.GetByPatternAsync<StationStatus>("station-status*");
        return Ok(result);
    }

}
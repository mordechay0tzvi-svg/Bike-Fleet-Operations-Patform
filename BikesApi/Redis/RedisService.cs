namespace Redis;
using System.Text.Json;
using StackExchange.Redis;
public class RedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    public RedisService(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _database = _redis.GetDatabase();
    }
    public async Task SetAsync<T>(string key, T value)
    {
        var set = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, set);
    }
    public async Task<T?> GetAsync<T>(string key)
    {
        var get = await _database.StringGetAsync(key);
        if (get.IsNullOrEmpty)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(get!);
    }
    public async Task ClearAsync()
    {
        var endpoints = _redis.GetEndPoints();
        foreach (var e in endpoints)
        {
            var server = _redis.GetServer(e);
            var keys = server.Keys(database: _database.Database,pattern: "station-status:*");
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
        }
    }
    public async Task<List<T>> GetByPatternAsync<T>(string pattern)
    {
        var result = new List<T>();
        var endpoints = _redis.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);
            foreach (var key in server.Keys(database: _database.Database, pattern: pattern))
            {
                var item = await GetAsync<T>(key!);
                if (item != null)
                {
                    result.Add(item);
                }
            }
        }
        return result;
    }

}


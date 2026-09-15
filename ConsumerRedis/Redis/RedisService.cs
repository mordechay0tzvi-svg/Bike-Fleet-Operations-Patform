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
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json);
    }
    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(json!);
    }
    public async Task ClearAsync()
    {
        var endpoints = _redis.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);
            var keys = server.Keys(database: _database.Database,pattern: "station-status:*");
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
        }
    }
}


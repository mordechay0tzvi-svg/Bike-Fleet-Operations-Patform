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
}


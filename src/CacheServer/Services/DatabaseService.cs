using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models;

namespace Clustache.Services;

public class DatabaseConfiguration
{
    public string ConnectionString { get; init; }
}

public class DatabaseService(ILogger<DatabaseService> logger, DatabaseConfiguration configuration)
{
    private readonly ILogger<DatabaseService> logger = logger;
    private readonly DatabaseConfiguration configuration = configuration;

    public async Task<CachedItem> GetItem(string key)
    {
        var uri = new Uri($"{configuration.ConnectionString}/item/{key}");
        logger.LogInformation($"Getting item from database: {uri.AbsolutePath}");
        var response = await new HttpClient().GetAsync(uri);
        var responseString = await response.Content.ReadAsStringAsync();
        logger.LogInformation($"Response: {responseString}");
        var item = JsonSerializer.Deserialize<CachedItem>(responseString);

        return item;
    }
}

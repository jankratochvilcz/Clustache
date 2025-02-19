using System.Text.Json.Serialization;

namespace Models;

public class CachedItem
{
    [JsonPropertyName("itemId")]
    public string ItemId { get; set; }

    [JsonPropertyName("itemValue")]
    public string ItemValue { get; set; }

    [JsonIgnore]
    public CachedItemOrigin Origin { get; set; }
}

public enum CachedItemOrigin
{
    Source = 1,
    RedisSubscription = 2,
}

namespace Models;

public class CachedItem
{
    public string ItemId { get; set; }
    public string ItemValue { get; set; }

    public CachedItemOrigin Origin { get; set; }
}

public enum CachedItemOrigin
{
    Source = 1,
    RedisSubscription = 2,
}

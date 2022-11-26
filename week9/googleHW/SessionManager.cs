namespace googleHW;

using Microsoft.Extensions.Caching.Memory;

public static class SessionManager
{
    private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());    

    public static Session CreateSession(object key, Func<Session> createItem)
    {
        Session cacheEntry;
        if (!_cache.TryGetValue(key, out cacheEntry)) // Ищем ключ в кэше.
        {
            // Ключ отсутствует в кэше, поэтому получаем данные.
            cacheEntry = createItem();
            
            // Сохраняем данные в кэше. 
            _cache.Set(key, cacheEntry, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(2)));
        }
        return cacheEntry;
    }

    public static bool CheckSession(object key)
    {
        return !_cache.TryGetValue(key, out _);
    }

    public static Session? GetSessionInfo(object key)
    {
        return _cache.TryGetValue(key, out Session session)? session: null;
    }
}

public class Session
{
    public static int Id { get; set; }
    
    public static int AccountId { get; set; }
    
    public static string Email { get; set; }
    
    public static DateTime CreateDateTime { get; set; }
}
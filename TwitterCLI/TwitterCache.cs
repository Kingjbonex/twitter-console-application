using Microsoft.Extensions.Caching.Memory;

namespace TwitterCLI;

public interface ITwitterCache
{
    IMemoryCache MemoryCache { get; }

    string TweetCountKey { get; }
    string HashtagsKey { get; }
}

public class TwitterCache : ITwitterCache
{
    public TwitterCache(IMemoryCache memoryCache)
    {
        MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public IMemoryCache MemoryCache { get; private set; }

    public string TweetCountKey => "Tweet Count";

    public string HashtagsKey => "Hashtags";
}

using Microsoft.Extensions.Caching.Memory;

namespace TwitterCLI
{
    public class TwitterCache
    {
        /// <summary>
        /// To be used for Unit Testing.
        /// </summary>
        public TwitterCache() { }

        public TwitterCache(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public IMemoryCache MemoryCache { get; private set; }
    }
}

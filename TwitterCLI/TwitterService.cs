using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Tweetinvi.Streaming.V2;

namespace TwitterCLI
{
    internal interface IScopedProcessingService
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }

    /// <summary>
    /// A background process that streams twitter data and saves it to an in-memory cache be displayed elsewhere.
    /// </summary>
    public class TwitterService : IScopedProcessingService
    {
        private ITwitterCache _memCache;
        private ILogger<TwitterService> _logger;
        private ISampleStreamV2 _sampleStream;
        private int tweetCount = 0;
        private Dictionary<string, int> hashTags = new Dictionary<string, int>();

        /// <summary>
        /// A 
        /// </summary>
        /// <param name="sampleStream">A Sample Stream from the V2 twitter API that is expected to not be started.</param>
        /// <param name="memCache">A Memory Cache to save the twitter stream data.</param>
        /// <param name="logger">Logger with the context of <see cref="TwitterService"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter passed in is null.</exception>
        public TwitterService(ISampleStreamV2 sampleStream, ITwitterCache memCache, ILogger<TwitterService> logger)
        {
            _sampleStream = sampleStream ?? throw new ArgumentNullException(nameof(sampleStream));
            _memCache = memCache ?? throw new ArgumentNullException(nameof(memCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Starts the <see cref="ISampleStreamV2"/> and listens to the Tweet Recieved event.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting the Twitter V2 Sample Stream");
            _sampleStream.TweetReceived += (sender, eventArgs) =>
            {
                tweetCount++;
                _memCache.MemoryCache.Set(_memCache.TweetCountKey, tweetCount);
                if (eventArgs.Tweet.Entities.Hashtags != null)
                {
                    foreach (Tweetinvi.Models.V2.HashtagV2 hashtag in eventArgs.Tweet.Entities.Hashtags)
                    {
                        if (hashTags.ContainsKey(hashtag.Tag))
                        {
                            hashTags[hashtag.Tag]++;
                        }
                        else
                        {
                            hashTags[hashtag.Tag] = 1;
                        }
                    }
                    _memCache.MemoryCache.Set(_memCache.HashtagsKey, hashTags);
                }
            };
            return _sampleStream.StartAsync();
        }
    }
}
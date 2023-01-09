using Microsoft.Extensions.Caching.Memory;
using Tweetinvi.Streaming.V2;

namespace TwitterCLI;

/// <summary>
/// A background process that streams twitter data and saves it to an in-memory cache be displayed elsewhere.
/// </summary>
public class TwitterService : IScopedProcessingService
{
    private ITwitterCache _memCache;
    private ITwitterClient _twitterClient;
    private int tweetCount = 0;
    private Dictionary<string, int> hashTags = new Dictionary<string, int>();

    /// <summary>
    /// A Background service that calls the Twitter Sample Stream V2 API
    /// </summary>
    /// <param name="twitterClient">A configured TwitterClient.</param>
    /// <param name="memCache">A Memory Cache to save the twitter stream data.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter passed in is null.</exception>
    public TwitterService(ITwitterClient twitterClient, ITwitterCache memCache)
    {
        _memCache = memCache ?? throw new ArgumentNullException(nameof(memCache));
        _twitterClient = twitterClient ?? throw new ArgumentNullException(nameof(twitterClient));
    }

    /// <summary>
    /// Gets and starts a Sample Stream v2 from Twitter and listens to the Tweet Recieved event.
    /// </summary>
    /// <param name="stoppingToken">A cancellation token used to stop the Stream.</param>
    /// <returns>A started <see cref="ISampleStreamV2"/>.</returns>
    public Task DoWorkAsync(CancellationToken stoppingToken)
    {
        var sampleStream = _twitterClient.GetSampleStreamV2();
        sampleStream.TweetReceived += (sender, eventArgs) =>
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
        return sampleStream.StartAsync();
    }
}
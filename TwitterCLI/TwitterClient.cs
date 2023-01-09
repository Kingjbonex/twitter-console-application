using Tweetinvi.Streaming.V2;

namespace TwitterCLI;

public class TwitterClient : ITwitterClient
{
    private Tweetinvi.TwitterClient _twitterClient;

    /// <summary>
    /// Creates a Sample Stream v2 from the Twitter API.
    /// </summary>
    /// <returns></returns>
    public ISampleStreamV2 GetSampleStreamV2()
    {
        return _twitterClient.StreamsV2.CreateSampleStream();
    }

    /// <summary>
    /// Sets the internal TwitterClient based off the <see cref="SampleCommand.Settings"/> passed into the CLI.
    /// </summary>
    /// <param name="settings">Settings object from the <see cref="SampleCommand"/>.</param>
    public void ConfigureTwitterClient(SampleCommand.Settings settings)
    {
        var creds = new Tweetinvi.Models.ConsumerOnlyCredentials(settings.TwitterApiKey, settings.TwitterSecretKey)
        {
            BearerToken = settings.TwitterBearerToken,
        };
        _twitterClient = new Tweetinvi.TwitterClient(creds);
    }
}

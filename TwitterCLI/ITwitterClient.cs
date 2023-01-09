using Tweetinvi.Streaming.V2;

namespace TwitterCLI;

public interface ITwitterClient
{
    ISampleStreamV2 GetSampleStreamV2();

    void ConfigureTwitterClient(SampleCommand.Settings settings);
}

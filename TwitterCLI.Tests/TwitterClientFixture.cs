using NUnit.Framework;
using System.Configuration;

namespace TwitterCLI.Tests;

[TestFixture]
public class TwitterClientFixture
{
    [Test]
    public void GetSampleStreamV2_ShouldThrow_WhenTwitterClientHasntBeenConfigured()
    {
        TwitterClient client = new TwitterClient();

        Assert.That(() =>
        {
            client.GetSampleStreamV2();
        }, Throws.TypeOf<ConfigurationErrorsException>());
    }
}

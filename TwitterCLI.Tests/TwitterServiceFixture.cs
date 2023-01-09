using Moq;
using NUnit.Framework;

namespace TwitterCLI.Tests;

[TestFixture]
public class TwitterServiceFixture
{
    [Test]
    public void Constructor_ShouldThrow_WhenTwitterClientsNull()
    {
        Mock<ITwitterCache> twitterCacheMock = new Mock<ITwitterCache>();

        Assert.That(() =>
        {
            return new TwitterService(null, twitterCacheMock.Object);
        }, Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Constructor_ShouldThrow_WhenTwitterCacheIsNull()
    {
        Mock<ITwitterClient> twitterClientMock = new Mock<ITwitterClient>();

        Assert.That(() =>
        {
            return new TwitterService(twitterClientMock.Object, null);
        }, Throws.TypeOf<ArgumentNullException>());
    }
}

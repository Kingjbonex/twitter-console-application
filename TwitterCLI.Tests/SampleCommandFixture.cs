using Moq;
using NUnit.Framework;

namespace TwitterCLI.Tests;

[TestFixture]
public class SampleCommandFixture
{
    [Test]
    public void Constructor_ShouldThrow_WhenBackgroundProcessingServiceIsNull()
    {
        Mock<ITwitterClient> twitterClientMock = new Mock<ITwitterClient>();
        Mock<ITwitterCache> twitterCacheMock = new Mock<ITwitterCache>();

        Assert.That(() =>
        {
            return new SampleCommand(null, twitterClientMock.Object, twitterCacheMock.Object);
        }, Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Constructor_ShouldThrow_WhenTwitterClientIsNull()
    {
        Mock<IScopedProcessingService> processMock = new Mock<IScopedProcessingService>();
        Mock<ITwitterCache> twitterCacheMock = new Mock<ITwitterCache>();

        Assert.That(() =>
        {
            return new SampleCommand(processMock.Object, null, twitterCacheMock.Object);
        }, Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Constructor_ShouldThrow_WhenTwitterCacheIsNull()
    {
        Mock<IScopedProcessingService> processMock = new Mock<IScopedProcessingService>();
        Mock<ITwitterClient> twitterClientMock = new Mock<ITwitterClient>();

        Assert.That(() =>
        {
            return new SampleCommand(processMock.Object, twitterClientMock.Object, null);
        }, Throws.TypeOf<ArgumentNullException>());
    }
}

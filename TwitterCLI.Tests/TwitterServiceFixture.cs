using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Tweetinvi.Streaming.V2;

namespace TwitterCLI.Tests
{
    [TestFixture]
    public class TwitterServiceFixture
    {
        [Test]
        public void Constructor_ShouldThrow_WhenSampleStreamIsNull()
        {
            Mock<TwitterCache> twitterCacheMock = new Mock<TwitterCache>();
            Mock<ILogger<TwitterService>> loggerMock = new Mock<ILogger<TwitterService>>();

            Assert.That(() =>
            {
                return new TwitterService(null, twitterCacheMock.Object, loggerMock.Object);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_ShouldThrow_WhenTwitterCacheIsNull()
        {
            Mock<ISampleStreamV2> sampleStreamV2CacheMock = new Mock<ISampleStreamV2>();
            Mock<ILogger<TwitterService>> loggerMock = new Mock<ILogger<TwitterService>>();

            Assert.That(() =>
            {
                return new TwitterService(sampleStreamV2CacheMock.Object, null, loggerMock.Object);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_ShouldThrow_WhenLoggerIsNull()
        {
            Mock<ISampleStreamV2> sampleStreamV2CacheMock = new Mock<ISampleStreamV2>();
            Mock<TwitterCache> twitterCacheMock = new Mock<TwitterCache>();

            Assert.That(() =>
            {
                return new TwitterService(sampleStreamV2CacheMock.Object, twitterCacheMock.Object, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task DoWorkAsync_DoesNotThrow_WhenAllDependenciesAreSet()
        {
            Mock<ISampleStreamV2> sampleStreamV2CacheMock = new Mock<ISampleStreamV2>();
            Mock<TwitterCache> twitterCacheMock = new Mock<TwitterCache>();
            Mock<ILogger<TwitterService>> loggerMock = new Mock<ILogger<TwitterService>>();

            var uut = new TwitterService(sampleStreamV2CacheMock.Object, twitterCacheMock.Object, loggerMock.Object);

            Assert.DoesNotThrowAsync(async () => { await uut.DoWorkAsync(CancellationToken.None); });
        }
    }
}

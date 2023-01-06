using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace TwitterCLI.Tests
{
    [TestFixture]
    public class ConsoleServiceFixture
    {
        [Test]
        public void Constructor_ShouldThrow_WhenServiceProviderIsNull()
        {
            Mock<ITwitterCache> twitterCacheMock = new Mock<ITwitterCache>();
            Mock<ILogger<ConsoleService>> loggerMock = new Mock<ILogger<ConsoleService>>();

            Assert.That(() =>
            {
                return new ConsoleService(null, twitterCacheMock.Object, loggerMock.Object);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_ShouldThrow_WhenTwitterCacheIsNull()
        {
            Mock<IServiceProvider> serviceProviderCacheMock = new Mock<IServiceProvider>();
            Mock<ILogger<ConsoleService>> loggerMock = new Mock<ILogger<ConsoleService>>();

            Assert.That(() =>
            {
                return new ConsoleService(serviceProviderCacheMock.Object, null, loggerMock.Object);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_ShouldThrow_WhenLoggerIsNull()
        {
            Mock<IServiceProvider> serviceProviderCacheMock = new Mock<IServiceProvider>();
            Mock<ITwitterCache> twitterCacheMock = new Mock<ITwitterCache>();

            Assert.That(() =>
            {
                return new ConsoleService(serviceProviderCacheMock.Object, twitterCacheMock.Object, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace TwitterCLI.Tests
{
    [TestFixture]
    public class TwitterCacheFixture
    {
        [Test]
        public void Constructor_ShouldThrow_WhenMemoryCacheIsNull()
        {
            Assert.That(() =>
            {
                return new TwitterCache(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}

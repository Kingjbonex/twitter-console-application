using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Tweetinvi.Streaming.V2;

namespace TwitterCLI.Tests
{
    [TestFixture]
    public class ProgramHostFixture
    {
        [Test]
        public void CreateHostBuilder_ShouldResolveAllServices_When()
        {
            var hostBuilder = Program.CreateHostBuilder(null);

            Assert.That(() => hostBuilder.Build().Services.GetService<ISampleStreamV2>(), Is.Not.Null);

        }
    }
}

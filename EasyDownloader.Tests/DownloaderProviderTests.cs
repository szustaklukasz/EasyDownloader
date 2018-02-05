using EasyDownloader.Utilities.Downloader;
using Moq;
using NUnit.Framework;
using System;

namespace EasyDownloader.Tests.Providers
{
    [TestFixture]
    public class DownloaderProviderTests
    {
        private readonly Mock<DownloaderProvider> _downloaderProvider;

        public DownloaderProviderTests()
        {
            _downloaderProvider = new Mock<DownloaderProvider>();
        }

        [Test]
        public void get_videos_should_throw_excepton_if_source_type_out_of_range()
        {
            DownloaderDTO downloaderDTO = new DownloaderDTO();

            Assert.Throws<AggregateException>(() => new DownloaderProvider(downloaderDTO).GetVideos().Wait());
        }
    }
}

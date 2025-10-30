namespace LinkLaunderer.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides unit tests for the LinkProcessor class to verify correct processing of various social media URLs.
    /// </summary>
    /// <remarks>
    /// Unfortunately these tests require internet access - in the future, I'd like to emulate/mock all or some of the http calls.
    /// </remarks>
    [TestClass]
    public sealed class LinkProcessorTests
    {
        [TestMethod]
        public async Task TikTokTest()
        {
            string url = "https://www.tiktok.com/@shophikikomori/video/7550414052816604429?_r=1&_t=ZT-90sHupOVCc1";
            var processor = new LinkLaunderer.Lib.LinkProcessor();
            string result = await processor.Process(url);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TikTokRedirTest()
        {
            string url = "https://www.tiktok.com/t/ZP8DFHpxu/";
            var processor = new LinkLaunderer.Lib.LinkProcessor();
            string result = await processor.Process(url);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task YoutubeStandard()
        {
            string url = "https://www.youtube.com/watch?v=kN8zXuhOsPA";
            var processor = new LinkLaunderer.Lib.LinkProcessor();
            string result = await processor.Process(url);
            Assert.IsNotNull(result);
            Assert.AreEqual(url, result, "Output url should match input");
        }

        [TestMethod]
        public async Task YoutubeShort()
        {
            string url = "https://youtu.be/9E1dytfmFxA";
            var processor = new LinkLaunderer.Lib.LinkProcessor();
            string result = await processor.Process(url);
            Assert.IsNotNull(result);
            Assert.AreEqual("https://www.youtube.com/watch?v=9E1dytfmFxA", result, "Output url does not match expected value");
        }
    }
}

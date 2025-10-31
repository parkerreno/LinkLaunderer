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
        /// <summary>
        /// Verifies that a full TikTok URL with query parameters is processed correctly without host replacement.
        /// </summary>
        [TestMethod]
        public async Task TikTok_FullWithQueryParams_NoReplacement()
        {
            string url = "https://www.tiktok.com/@shophikikomori/video/7550414052816604429?_r=1&_t=ZT-90sHupOVCc1";
            var processor = new LinkLaunderer.Lib.LinkProcessor(TestHelpers.DefaultLinkProcessorOptionsWithoutReplacements);
            Uri result = await processor.Process(url);

            Assert.IsNotNull(result);
            Assert.AreEqual("www.tiktok.com", result.Host, "Host should remain unchanged");
            Assert.AreEqual("https://www.tiktok.com/@shophikikomori/video/7550414052816604429", result.ToString(), "Output url does not match expected value");
        }

        /// <summary>
        /// Verifies that a TikTok short link is expanded to the full URL without host replacement.
        /// </summary>
        [TestMethod]
        public async Task TikTok_ShortLink_NoReplacement()
        {
            string url = "https://www.tiktok.com/t/ZP8DFHpxu/";
            var processor = new LinkLaunderer.Lib.LinkProcessor(TestHelpers.DefaultLinkProcessorOptionsWithoutReplacements);
            Uri result = await processor.Process(url);

            Assert.IsNotNull(result);
            Assert.AreEqual("www.tiktok.com", result.Host, "Host should remain unchanged");
            Assert.AreEqual("https://www.tiktok.com/@jaimewash/video/7558813645413600525", result.ToString(), "Output url does not match expected value");
        }

        /// <summary>
        /// The TikTok home page redirects to a relative link, rather than an absolute link, so this test ensures that
        /// edge case is handled correctly.
        /// </summary>
        [TestMethod]
        public async Task TikTok_HomePage_NoReplacement()
        {
            string url = "https://www.tiktok.com/";
            var processor = new LinkLaunderer.Lib.LinkProcessor(TestHelpers.DefaultLinkProcessorOptionsWithoutReplacements);
            Uri result = await processor.Process(url);

            Assert.IsNotNull(result);
            Assert.AreEqual("www.tiktok.com", result.Host, "Host should remain unchanged");
            Assert.AreEqual("https://www.tiktok.com/explore", result.ToString(), "Output url does not match expected value");
        }

        /// <summary>
        /// Verifies that a full TikTok URL with query parameters is processed correctly with host replacement to sticktock.com.
        /// </summary>
        [TestMethod]
        public async Task TikTok_FullWithQueryParams_WithReplacement()
        {
            string url = "https://www.tiktok.com/@shophikikomori/video/7550414052816604429?_r=1&_t=ZT-90sHupOVCc1";
            var processor = new LinkLaunderer.Lib.LinkProcessor();
            Uri result = await processor.Process(url);

            Assert.IsNotNull(result);
            Assert.AreEqual("sticktock.com", result.Host, "Host should remain unchanged");
            Assert.AreEqual("https://sticktock.com/@shophikikomori/video/7550414052816604429", result.ToString(), "Output url does not match expected value");
        }

        /// <summary>
        /// Verifies that a TikTok short link is expanded and host is replaced with sticktock.com.
        /// </summary>
        [TestMethod]
        public async Task TikTok_ShortLink_WithReplacement()
        {
            string url = "https://www.tiktok.com/t/ZP8DFHpxu/";
            var processor = new LinkLaunderer.Lib.LinkProcessor();
            Uri result = await processor.Process(url);

            Assert.IsNotNull(result);
            Assert.AreEqual("sticktock.com", result.Host, "Host should remain unchanged");
            Assert.AreEqual("https://sticktock.com/@jaimewash/video/7558813645413600525", result.ToString(), "Output url does not match expected value");
        }

        /// <summary>
        /// Verifies that a full YouTube URL without query parameters is processed correctly without modification.
        /// </summary>
        [TestMethod]
        public async Task Youtube_FullLinkNoParams()
        {
            string url = "https://www.youtube.com/watch?v=kN8zXuhOsPA";
            var processor = new LinkLaunderer.Lib.LinkProcessor();
            Uri result = await processor.Process(url);

            Assert.IsNotNull(result);
            Assert.AreEqual("www.youtube.com", result.Host, "Host should remain unchanged");
            Assert.AreEqual(url, result.ToString(), "Output url should match input");
        }
        
        /// <summary>
        /// Verifies that a full YouTube URL with query parameters has the extra parameters removed.
        /// </summary>
        [TestMethod]
        public async Task Youtube_FullLinkWithParams()
        {
            string url = "https://www.youtube.com/watch?v=5wxF7pAQqdc&feature=youtu.be";
            var processor = new LinkLaunderer.Lib.LinkProcessor();
            Uri result = await processor.Process(url);

            Assert.IsNotNull(result);
            Assert.AreEqual("www.youtube.com", result.Host, "Host should remain unchanged");
            Assert.AreEqual("https://www.youtube.com/watch?v=5wxF7pAQqdc", result.ToString(), "Output url does not match expected value.");
        }

        /// <summary>
        /// Verifies that a YouTube short link (youtu.be) without parameters is expanded to the full youtube.com URL.
        /// </summary>
        [TestMethod]
        public async Task Youtube_ShortLinkNoParams()
        {
            string url = "https://youtu.be/9E1dytfmFxA";
            var processor = new LinkLaunderer.Lib.LinkProcessor();
            Uri result = await processor.Process(url);

            Assert.IsNotNull(result);
            Assert.AreEqual("www.youtube.com", result.Host, "Host should be youtube.com (redirected location).");
            Assert.AreEqual("https://www.youtube.com/watch?v=9E1dytfmFxA", result.ToString(), "Output url does not match expected value");
        }
        
        /// <summary>
        /// Verifies that a YouTube short link (youtu.be) with parameters is expanded to the full youtube.com URL with parameters removed.
        /// </summary>
        [TestMethod]
        public async Task Youtube_ShortLinkWithParams()
        {
            string url = "https://youtu.be/5wxF7pAQqdc?si=wmWFr92_Qz02J6q8";
            var processor = new LinkLaunderer.Lib.LinkProcessor();
            Uri result = await processor.Process(url);

            Assert.IsNotNull(result);
            Assert.AreEqual("www.youtube.com", result.Host, "Host should be youtube.com (redirected location).");
            Assert.AreEqual("https://www.youtube.com/watch?v=5wxF7pAQqdc", result.ToString(), "Output url does not match expected value");
        }
    }
}

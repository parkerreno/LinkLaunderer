namespace LinkLaunderer.Lib
{
    /// <summary>
    /// Options for configuring the LinkProcessor.
    /// </summary>
    public class LinkProcessorOptions
    {
        /// <summary>
        /// A list of domain replacements to apply during link processing.
        /// </summary>
        public required Dictionary<string, string> DomainReplacements { get; set; }

        /// <summary>
        /// Whether to strip the query parameters from links when processing.
        /// </summary>
        public bool RemoveQueryParameters { get; set; } = true;

        /// <summary>
        /// Allowed parameters when <see cref="RemoveQueryParameters"/> is true.
        /// </summary>
        public required List<string> AllowedParameters { get; set; }

        /// <summary>
        /// Creates a new instance of LinkProcessorOptions initialized with default values for domain replacements,
        /// allowed parameters, and query parameter removal settings.
        /// </summary>
        /// <remarks>The returned options include predefined domain replacements for common social media
        /// domains, a set of allowed query parameters, and a setting to remove query parameters by default. These
        /// defaults are intended to provide sensible behavior for typical link processing scenarios.</remarks>
        /// <returns>A LinkProcessorOptions object containing the default configuration for link processing operations.</returns>
        public static LinkProcessorOptions DefaultOptions()
        {
            return new LinkProcessorOptions()
            {
                DomainReplacements = new Dictionary<string, string>()
                {
                    { "x.com", "xcancel.com" },
                    { "twitter.com", "xcancel.com" },
                    { "tiktok.com", "sticktock.com" },
                    { "www.tiktok.com", "sticktock.com" },
                },
                AllowedParameters = new List<string>()
                {
                    "v",
                },
                RemoveQueryParameters = true,
            };
        }
    }
}

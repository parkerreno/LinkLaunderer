namespace LinkLaunderer.Lib
{
    using System.Collections.ObjectModel;
    using System.Text.Json;

    /// <summary>
    /// Options for configuring the LinkProcessor.
    /// </summary>
    public class LinkProcessorOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether domain names in the input should be replaced during processing.
        /// </summary>
        public bool ReplaceDomains { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether domain matching should include the 'www' subdomain.
        /// </summary>
        /// <remarks>When enabled, domain comparisons will treat domains with and without the 'www' prefix
        /// as equivalent. Disable this property to require exact matches between domains, including the presence or
        /// absence of 'www'.</remarks>
        public bool IncludeWwwInDomainMatching { get; set; } = true;

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
        public required ObservableCollection<string> AllowedParameters { get; set; }

        /// <summary>
        /// Saves the current configuration settings to application preferences.
        /// </summary>
        public void SaveToPreferences()
        {
            Preferences.Set(Constants.Preferences.ReplaceDomains, this.ReplaceDomains);
            Preferences.Set(Constants.Preferences.RemoveQueryParameters, this.RemoveQueryParameters);
            Preferences.Set(Constants.Preferences.WwwMatching, this.IncludeWwwInDomainMatching);
            Preferences.Set(Constants.Preferences.DomainReplacements, JsonSerializer.Serialize(this.DomainReplacements));
            Preferences.Set(Constants.Preferences.AllowedParameters, JsonSerializer.Serialize(this.AllowedParameters));
        }

        /// <summary>
        /// Loads a new instance of the LinkProcessorOptions class using the current application preferences.
        /// </summary>
        /// <returns>A LinkProcessorOptions object populated with values retrieved from the application's preferences. Default
        /// values are used for any preferences that are not set.</returns>
        public static LinkProcessorOptions LoadFromPreferences()
        {
            Dictionary<string, string>? replacements = null;
            ObservableCollection<string>? allowedParams = null;
            bool removeParams = Preferences.Get(Constants.Preferences.RemoveQueryParameters, true);
            bool wwwMatching = Preferences.Get(Constants.Preferences.WwwMatching, true);
            bool replaceDomains = Preferences.Get(Constants.Preferences.ReplaceDomains, true);

            if (Preferences.ContainsKey(Constants.Preferences.DomainReplacements))
            {
                string json = Preferences.Get(Constants.Preferences.DomainReplacements, string.Empty);
                replacements = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? null;
            }

            if (Preferences.ContainsKey(Constants.Preferences.AllowedParameters))
            {
                string json = Preferences.Get(Constants.Preferences.AllowedParameters, string.Empty);
                allowedParams = JsonSerializer.Deserialize<ObservableCollection<string>>(json) ?? null;
            }

            LinkProcessorOptions options = LinkProcessorOptions.DefaultOptions();
            options.AllowedParameters = allowedParams ?? options.AllowedParameters;
            options.DomainReplacements = replacements ?? options.DomainReplacements;
            options.DomainReplacements = new Dictionary<string, string>(options.DomainReplacements, StringComparer.OrdinalIgnoreCase);
            options.RemoveQueryParameters = removeParams;
            options.IncludeWwwInDomainMatching = wwwMatching;
            options.ReplaceDomains = replaceDomains;

            return options;
        }

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
                ReplaceDomains = true,
                IncludeWwwInDomainMatching = true,
                DomainReplacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "x.com", "xcancel.com" },
                    { "twitter.com", "xcancel.com" },
                    { "tiktok.com", "sticktock.com" },
                },
                AllowedParameters = new ObservableCollection<string>()
                {
                    "v",
                },
                RemoveQueryParameters = true,
            };
        }
    }
}

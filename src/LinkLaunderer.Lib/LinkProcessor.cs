namespace LinkLaunderer.Lib
{
    using System.Collections.Specialized;
    using System.Web;

    /// <summary>
    /// Provides functionality to process URLs.
    /// </summary>
    public class LinkProcessor : IDisposable
    {
        private readonly LinkProcessorOptions options;
        private readonly HttpClient httpClient;

        /// <summary>
        /// Creates a new instance of the LinkProcessor with default options.
        /// </summary>
        public LinkProcessor() : this(LinkProcessorOptions.DefaultOptions()) { }

        /// <summary>
        /// Initializes a new instance of the LinkProcessor class using the specified options.
        /// </summary>
        /// <param name="options">The configuration options to use for processing links. Cannot be null.</param>
        public LinkProcessor(LinkProcessorOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            this.httpClient = InitializeHttpClient();
            this.options = options;
        }

        /// <summary>
        /// Initializes a new instance of the HttpClient class configured for use in the <see cref="LinkProcessor"/>.
        /// </summary>
        /// <remarks>Use this client when you need to handle HTTP redirect responses manually, such as
        /// when you want to inspect or process redirect URLs yourself. The returned HttpClient uses a handler with
        /// AllowAutoRedirect set to false.</remarks>
        /// <returns>A HttpClient instance that does not automatically follow HTTP redirect responses.</returns>
        private static HttpClient InitializeHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false, // We want to get the redirect URL ourselves
            };

            return new HttpClient(handler);
        }

        /// <summary>
        /// Processes the specified URL and returns the result as a string.
        /// </summary>
        /// <param name="url">The URL to process. Must be a valid absolute or relative URI string.</param>
        /// <returns>A string containing the result of processing the specified URL.</returns>
        public async Task<Uri> Process(string url)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(url);

            return await this.Process(new Uri(url));
        }

        /// <summary>
        /// Processes the specified URI and returns a result string representing the outcome of the operation.
        /// </summary>
        /// <param name="uri">The URI to be processed. Cannot be null.</param>
        /// <returns>A string containing the result of processing the specified URI. Returns an empty string if no result is
        /// produced.</returns>
        public async Task<Uri> Process(Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            HttpResponseMessage response = await this.httpClient.GetAsync(uri).ConfigureAwait(true);
            Uri? redirectedLocation = response.Headers.Location;

            if (redirectedLocation != null)
            {
                if (redirectedLocation.IsAbsoluteUri)
                {
                    uri = redirectedLocation;
                }
                else
                {
                    UriBuilder combinedUriBuilder = new(uri.Host);
                    combinedUriBuilder.Scheme = uri.Scheme;
                    combinedUriBuilder.Path = redirectedLocation.ToString();
                    uri = combinedUriBuilder.Uri;
                }
            }

            UriBuilder newUriBuilder = new();
            newUriBuilder.Scheme = uri.Scheme;
            newUriBuilder.Path = uri.AbsolutePath;

            /* Deal with query parameters */
            if (!this.options.RemoveQueryParameters)
            {
                newUriBuilder.Query = uri.Query;
            }
            else if (this.options.AllowedParameters.Count > 0)
            {
                NameValueCollection originalQueryParams = HttpUtility.ParseQueryString(uri.Query);
                NameValueCollection filteredParams = HttpUtility.ParseQueryString(string.Empty);
                string?[] keys = originalQueryParams.AllKeys;
                
                foreach (string allowedParamKey in this.options.AllowedParameters)
                {
                    if (keys.Contains(allowedParamKey))
                    {
                        filteredParams.Set(allowedParamKey, originalQueryParams[allowedParamKey]);
                    }
                }

                newUriBuilder.Query = filteredParams.ToString() ?? string.Empty;
            }

            /* Deal with domain replacements */
            if (this.options.ReplaceDomains
                && this.options.DomainReplacements.TryGetValue(uri.Host, out string? replacementDomain))
            {
                newUriBuilder.Host = replacementDomain;
            }
            else if (this.options.ReplaceDomains
                && this.options.IncludeWwwInDomainMatching
                && uri.Host.StartsWith("www.")
                && this.options.DomainReplacements.TryGetValue(uri.Host.Substring(4), out replacementDomain))
            {
                newUriBuilder.Host = replacementDomain;
            }
            else
            {
                newUriBuilder.Host = uri.Host;
            }

            return newUriBuilder.Uri;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}

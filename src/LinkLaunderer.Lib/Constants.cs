namespace LinkLaunderer.Lib
{
    /// <summary>
    /// Provides a container for application-wide constant values.
    /// </summary>
    internal class Constants
    {
        /// <summary>
        /// Keys used for persisting user preferences.
        /// </summary>
        internal class Preferences
        {
            internal const string ReplaceDomains = "ReplaceDomains";
            internal const string RemoveQueryParameters = "RemoveQueryParameters";
            internal const string WwwMatching = "WwwMatching";
            internal const string DomainReplacements = "DomainReplacements";
            internal const string AllowedParameters = "AllowedParameters";
        }
    }
}

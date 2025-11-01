namespace LinkLaunderer.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using LinkLaunderer.Lib;

    internal class TestHelpers
    {
        /// <summary>
        /// Gets the default link processor options with domain replacements cleared.
        /// </summary>
        internal static LinkProcessorOptions DefaultLinkProcessorOptionsWithoutReplacements
        {
            get
            {
                if (field is null)
                {
                    LinkProcessorOptions options = LinkProcessorOptions.DefaultOptions();
                    options.DomainReplacements?.Clear();

                    field = options;
                }

                return field;
            }
        }
    }
}

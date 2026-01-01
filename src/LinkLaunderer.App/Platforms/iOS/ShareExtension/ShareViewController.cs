namespace LinkLaunderer
{
    using Foundation;
    using LinkLaunderer.Lib;
    using UIKit;
    using UniformTypeIdentifiers;

    /// <summary>
    /// Share extension view controller for iOS that processes shared URLs and text.
    /// </summary>
    [Register("ShareViewController")]
    public class ShareViewController : UIViewController
    {
        public ShareViewController(IntPtr handle) : base(handle)
        {
        }

        /// <inheritdoc/>
        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Get the extension context
            NSExtensionContext? context = this.ExtensionContext;
            if (context == null)
            {
                this.CompleteRequest();
                return;
            }

            // Process the shared content
            await this.ProcessSharedContent(context);
        }

        /// <summary>
        /// Processes the shared content from the extension context.
        /// </summary>
        /// <param name="context">The extension context containing the shared items.</param>
        private async Task ProcessSharedContent(NSExtensionContext context)
        {
            try
            {
                NSExtensionItem[]? items = context.InputItems;
                if (items == null || items.Length == 0)
                {
                    this.CompleteRequest();
                    return;
                }

                foreach (NSExtensionItem item in items)
                {
                    NSItemProvider[]? attachments = item.Attachments;
                    if (attachments == null || attachments.Length == 0)
                    {
                        continue;
                    }

                    foreach (NSItemProvider provider in attachments)
                    {
                        // Try to get URL first
                        if (provider.HasItemConformingTo(UTTypes.Url))
                        {
                            NSObject? urlObj = await provider.LoadItemAsync(UTTypes.Url, null);
                            if (urlObj is NSUrl nsUrl && nsUrl.AbsoluteString != null)
                            {
                                await this.HandleSharedText(nsUrl.AbsoluteString);
                                return;
                            }
                        }

                        // Try to get plain text
                        if (provider.HasItemConformingTo(UTTypes.PlainText))
                        {
                            NSObject? textObj = await provider.LoadItemAsync(UTTypes.PlainText, null);
                            if (textObj is NSString nsString && !string.IsNullOrEmpty(nsString.ToString()))
                            {
                                await this.HandleSharedText(nsString.ToString());
                                return;
                            }
                        }
                    }
                }

                this.CompleteRequest();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing shared content: {ex.Message}");
                this.CompleteRequest();
            }
        }

        /// <summary>
        /// Handles the shared text by processing it with LinkProcessor and sharing the result.
        /// </summary>
        /// <param name="sharedText">The shared text or URL to process.</param>
        private async Task HandleSharedText(string sharedText)
        {
            try
            {
                using (LinkProcessor linkProcessor = new LinkProcessor(LinkProcessorOptions.LoadFromPreferences()))
                {
                    Uri newUrl = await linkProcessor.Process(sharedText).ConfigureAwait(true);
                    
                    // Create a share sheet with the cleaned URL
                    UIActivityViewController activityViewController = new UIActivityViewController(
                        new NSObject[] { NSUrl.FromString(newUrl.ToString()) },
                        null);

                    // Present the share sheet
                    await this.PresentViewControllerAsync(activityViewController, true);

                    // Wait a bit for user interaction, then complete
                    // The completion handler will be called when user dismisses the share sheet
                    activityViewController.SetCompletionHandler((activityType, completed) =>
                    {
                        this.CompleteRequest();
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling shared text: {ex.Message}");
                this.CompleteRequest();
            }
        }

        /// <summary>
        /// Completes the extension request and dismisses the extension UI.
        /// </summary>
        private void CompleteRequest()
        {
            this.ExtensionContext?.CompleteRequest(new NSExtensionItem[0], null);
        }
    }
}

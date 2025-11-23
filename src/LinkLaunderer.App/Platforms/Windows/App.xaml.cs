// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LinkLaunderer.WinUI
{
    using LinkLaunderer.Lib;
    using Microsoft.Maui.Platform;
    using Microsoft.UI.Xaml;
    using Windows.ApplicationModel.Activation;
    using Windows.ApplicationModel.DataTransfer.ShareTarget;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        private ShareOperation? pendingShareOperation;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Check if the app was activated as a share target
            var activationArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

            if (activationArgs != null && activationArgs.Kind == Microsoft.Windows.AppLifecycle.ExtendedActivationKind.ShareTarget)
            {
                var shareArgs = activationArgs.Data as ShareTargetActivatedEventArgs;
                if (shareArgs != null)
                {
                    pendingShareOperation = shareArgs.ShareOperation;
                    // Continue with normal MAUI initialization
                    base.OnLaunched(args);
                    
                    // Handle the share operation after MAUI is initialized
                    if (MauiContext != null)
                    {
                        _ = HandleShareAsync(pendingShareOperation);
                    }
                    return;
                }
            }

            base.OnLaunched(args);
        }

        private async Task HandleShareAsync(ShareOperation shareOperation)
        {
            try
            {
                string? sharedText = null;

                // Try to get text data
                if (shareOperation.Data.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
                {
                    sharedText = await shareOperation.Data.GetTextAsync();
                }
                // Try to get web link
                else if (shareOperation.Data.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.WebLink))
                {
                    var webLink = await shareOperation.Data.GetWebLinkAsync();
                    sharedText = webLink?.ToString();
                }

                if (!string.IsNullOrEmpty(sharedText))
                {
                    // Process the shared text/URL
                    using (LinkProcessor linkProcessor = new LinkProcessor(LinkProcessorOptions.LoadFromPreferences()))
                    {
                        Uri newUrl = await linkProcessor.Process(sharedText).ConfigureAwait(true);
                        
                        // Copy to clipboard as a fallback
                        await Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.SetTextAsync(newUrl.ToString());

                        // Try to share the processed URL
                        await Microsoft.Maui.ApplicationModel.DataTransfer.Share.Default.RequestAsync(new Microsoft.Maui.ApplicationModel.DataTransfer.ShareTextRequest
                        {
                            Text = newUrl.ToString(),
                        }).ConfigureAwait(true);
                    }

                    shareOperation.ReportCompleted();
                }
                else
                {
                    shareOperation.ReportError("No text or link found in shared data");
                }
            }
            catch (Exception ex)
            {
                shareOperation.ReportError($"Error processing link: {ex.Message}");
            }
            finally
            {
                // Close the app after handling the share
                Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread()?.TryEnqueue(() =>
                {
                    (this as IApplication)?.CloseWindow(this.Windows[0]);
                });
            }
        }
    }

}

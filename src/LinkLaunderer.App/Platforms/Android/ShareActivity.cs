namespace LinkLaunderer
{
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;
    using LinkLaunderer.Lib;

    [Activity(
        Theme = "@style/Maui.SplashTheme",
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
        Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionSend },
        Categories = new[] { Intent.CategoryDefault },
        DataMimeType = "text/plain")]
    public class ShareActivity : MauiAppCompatActivity
    {
        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Handle the incoming share intent
            if (Intent.ActionSend.Equals(Intent?.Action) && Intent.Type?.StartsWith("text/") == true)
            {
                string? sharedText = Intent.GetStringExtra(Intent.ExtraText);

                if (!string.IsNullOrEmpty(sharedText))
                {
                    // Process the shared text/URL
                    await HandleSharedText(sharedText);
                }
            }
            else
            {
                Finish();
            }
        }

        private async Task HandleSharedText(string sharedText)
        {
            using (LinkProcessor linkProcessor = new LinkProcessor())
            {
                string newUrl = await linkProcessor.Process(sharedText).ConfigureAwait(true);
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = newUrl,
                }).ConfigureAwait(true);

                Finish();
            }
        }
    }
}

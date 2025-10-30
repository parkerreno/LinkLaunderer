namespace LinkLaunderer
{
    using LinkLaunderer.Lib;

    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            CheckForSharedContent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckForSharedContent();
        }

        private void CheckForSharedContent()
        {
            if (Preferences.ContainsKey("SharedText"))
            {
                var sharedText = Preferences.Get("SharedText", string.Empty);
                if (!string.IsNullOrEmpty(sharedText))
                {
                    // Clear the preference so we don't process it again
                    Preferences.Remove("SharedText");

                    // Process the shared text/URL
                    ProcessSharedText(sharedText);
                }
            }
        }

        private void ProcessSharedText(string text)
        {
            // TODO: Implement your link processing logic here
            // For example, populate an entry field or automatically process the URL
            // You can use your LinkProcessor class from LinkLaunderer.Lib
        }

        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            string url = "https://www.tiktok.com/@shophikikomori/video/7550414052816604429?_r=1&_t=ZT-90sHupOVCc1";
            var processor = new LinkProcessor();
            string result = await processor.Process(url);

            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}

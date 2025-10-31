namespace LinkLaunderer
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using LinkLaunderer.Lib;

    public partial class MainPage : ContentPage
    {
        LinkProcessorOptions options;

        public MainPage()
        {
            InitializeComponent();
            LoadPreferences();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadPreferences();
        }

        private async void OnSaveClicked(object? sender, EventArgs e)
        {
            options.SaveToPreferences();
        }

        private void RemoveQuery_Toggled(object sender, ToggledEventArgs e)
        {
            options.RemoveQueryParameters = e.Value;
        }

        private void HostReplacement_Toggled(object sender, ToggledEventArgs e)
        {
            options.ReplaceDomains = e.Value;
        }

        private void WwwMatch_Toggled(object sender, ToggledEventArgs e)
        {
            options.IncludeWwwInDomainMatching = e.Value;
        }

        private void LoadPreferences()
        {
            // Yes all of this should be MVVM bindings, but this is a MVP.  Might consider in future.
            options = LinkProcessorOptions.LoadFromPreferences();
            RemoveQueryParametersSetting.IsToggled = options.RemoveQueryParameters;
            ReplaceLinkHostsSetting.IsToggled = options.ReplaceDomains;
            WwwHostSetting.IsToggled = options.IncludeWwwInDomainMatching;
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            string settingsJson = "https://github.com/parkerreno/linklaunderer";
            bool copy = await DisplayAlertAsync(
                title: "About LinkLaunderer",
                message: settingsJson,
                accept: "Copy to Clipboard",
                cancel: "Close");

            if (copy)
            {
                await Clipboard.SetTextAsync(settingsJson);
            }
        }

        private async void OnPrintClicked(object sender, EventArgs e)
        {
            string settingsJson = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
            bool copy = await DisplayAlertAsync(
                title: "Settings Debug",
                message: settingsJson,
                accept: "Copy to Clipboard",
                cancel: "Close");

            if (copy)
            {
                await Clipboard.SetTextAsync(settingsJson);
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            options = LinkProcessorOptions.DefaultOptions();
            options.SaveToPreferences();
            LoadPreferences();
        }
    }
}

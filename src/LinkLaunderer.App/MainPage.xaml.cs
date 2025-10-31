namespace LinkLaunderer
{
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
            options = LinkProcessorOptions.LoadFromPreferences();
            RemoveQueryParametersSetting.IsToggled = options.RemoveQueryParameters;
            ReplaceLinkHostsSetting.IsToggled = options.ReplaceDomains;
            WwwHostSetting.IsToggled = options.IncludeWwwInDomainMatching;
        }
    }
}

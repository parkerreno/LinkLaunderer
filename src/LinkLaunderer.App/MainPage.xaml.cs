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
            this.InitializeComponent();
            this.LoadPreferences();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.LoadPreferences();
        }

        private async void OnSaveClicked(object? sender, EventArgs e)
        {
            this.options.SaveToPreferences();
        }

        private void RemoveQuery_Toggled(object sender, ToggledEventArgs e)
        {
            this.options.RemoveQueryParameters = e.Value;
            this.DeterminAllowedParamsVisibility();
        }

        private void HostReplacement_Toggled(object sender, ToggledEventArgs e)
        {
            this.options.ReplaceDomains = e.Value;
            this.DetermineHostPreferenceVisibility();
        }

        private void WwwMatch_Toggled(object sender, ToggledEventArgs e)
        {
            this.options.IncludeWwwInDomainMatching = e.Value;
        }

        private void LoadPreferences()
        {
            // Yes all of this should be MVVM bindings, but this is a MVP.  Might consider in future.
            this.options = LinkProcessorOptions.LoadFromPreferences();
            this.RemoveQueryParametersSetting.IsToggled = this.options.RemoveQueryParameters;
            this.ReplaceLinkHostsSetting.IsToggled = this.options.ReplaceDomains;
            this.WwwHostSetting.IsToggled = this.options.IncludeWwwInDomainMatching;
            this.HostReplacementList.ItemsSource = this.options.DomainReplacements;
            this.AllowedParamsList.ItemsSource = this.options.AllowedParameters;
            this.DetermineHostPreferenceVisibility();
            this.DeterminAllowedParamsVisibility();
        }

        private async void DetermineHostPreferenceVisibility()
        {
            if (this.options.ReplaceDomains)
            {
                this.WwwHostSetting.IsVisible = true;
                this.HostReplacementList.IsVisible = true;

                // WaitAll to get these to fade in parallel
                await Task.WhenAll([this.WwwHostSetting.FadeToAsync(255), this.HostReplacementList.FadeToAsync(255)]);
            }
            else
            {
                // WaitAll to get these to fade in parallel
                await Task.WhenAll([this.WwwHostSetting.FadeToAsync(0), this.HostReplacementList.FadeToAsync(0)]);

                this.WwwHostSetting.IsVisible = false;
                this.HostReplacementList.IsVisible = false;
            }
        }

        private async void DeterminAllowedParamsVisibility()
        {
            if (this.options.RemoveQueryParameters)
            {
                this.AllowedParamsList.IsVisible = true;

                // WaitAll to get these to fade in parallel
                await Task.WhenAll([this.AllowedParamsList.FadeToAsync(255)]);
            }
            else
            {
                // WaitAll to get these to fade in parallel
                await Task.WhenAll([this.AllowedParamsList.FadeToAsync(0)]);

                this.AllowedParamsList.IsVisible = false;
            }
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            string settingsJson = "https://github.com/parkerreno/linklaunderer";
            bool copy = await this.DisplayAlertAsync(
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
            string settingsJson = JsonSerializer.Serialize(this.options, new JsonSerializerOptions { WriteIndented = true });
            bool copy = await this.DisplayAlertAsync(
                title: "Settings Debug",
                message: settingsJson,
                accept: "Copy to Clipboard",
                cancel: "Close");

            if (copy)
            {
                await Clipboard.SetTextAsync(settingsJson);
            }
        }

        private async void OnResetClicked(object sender, EventArgs e)
        {
            bool reset = await this.DisplayAlertAsync(
                title: "Reset Settings",
                message: "Reset all settings to application defaults?",
                accept: "Reset",
                cancel: "Cancel");

            if (reset)
            {
                this.options = LinkProcessorOptions.DefaultOptions();
                this.options.SaveToPreferences();
                this.LoadPreferences();
            }
        }

        private void OnDeleteHostReplacementClicked(object sender, EventArgs e)
        {
            Button? buttonSender = sender as Button;
            if (buttonSender != null)
            {
                string? key = (buttonSender.BindingContext as KeyValuePair<string, string>?)?.Key;

                if (key != null)
                {
                    this.options.DomainReplacements.Remove(key);

                    // This is a hack due to not doing proper binding with an observable collection
                    this.HostReplacementList.ItemsSource = null;
                    this.HostReplacementList.ItemsSource = this.options.DomainReplacements;
                }
            }
        }

        private async void AddReplacementClicked(object sender, EventArgs e)
        {
            string host = await this.DisplayPromptAsync(
                title: "Host Name",
                message: "The host name to replace");

            if (string.IsNullOrWhiteSpace(host))
            {
                return;
            }

            string replacement = await this.DisplayPromptAsync(
                title: "Replacement",
                message: "Replacement host");

            if (string.IsNullOrEmpty(replacement))
            {
                return;
            }

            if (this.options.DomainReplacements.ContainsKey(host))
            {
                if (!await this.DisplayAlertAsync(
                    title: "Host Already Exists",
                    message: "This hostname is already exists - replace entry?",
                    accept: "Replace",
                    cancel: "Cancel"))
                {
                    return;
                }
            }

            this.options.DomainReplacements[host] = replacement;

            // This is a hack due to not doing proper binding with an observable collection
            this.HostReplacementList.ItemsSource = null;
            this.HostReplacementList.ItemsSource = this.options.DomainReplacements;
        }

        private async void AddParamClicked(object sender, EventArgs e)
        {
            string param = await this.DisplayPromptAsync(
                title: "Allowed Parameter",
                message: "The query parameter to allow (without the ? or &)");

            if (string.IsNullOrWhiteSpace(param))
            {
                return;
            }

            if (!this.options.AllowedParameters.Contains(param))
            {
                this.options.AllowedParameters.Add(param);
            }
        }

        private void OnDeleteParamClicked(object sender, EventArgs e)
        {
            Button? buttonSender = sender as Button;
            if (buttonSender != null)
            {
                string? key = (buttonSender.BindingContext as string);

                if (key != null)
                {
                    this.options.AllowedParameters.Remove(key);
                }
            }
        }
    }
}

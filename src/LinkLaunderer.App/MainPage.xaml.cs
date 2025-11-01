namespace LinkLaunderer
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using LinkLaunderer.Lib;

    public partial class MainPage : ContentPage
    {
        LinkProcessorOptions options;

        /// <summary>
        /// Initializes a new instance of the MainPage class and loads user preferences.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.LoadPreferences();
        }

        /// <inheritdoc/>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.LoadPreferences();
        }

        /// <summary>
        /// Handles loading preferences from persistent storage and updating the UI elements to reflect the current settings.
        /// </summary>
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
            this.DetermineAllowedParamsVisibility();
        }

        /// <summary>
        /// Handles the save action by persisting the current options to user preferences.
        /// </summary>
        /// <param name="sender">The source of the event, typically the control that initiated the save action.</param>
        /// <param name="e">An EventArgs instance containing event data.</param>
        private void OnSaveClicked(object? sender, EventArgs e)
        {
            this.options.SaveToPreferences();
        }

        /// <summary>
        /// Handles the toggling of the option to remove query parameters when the associated UI element is toggled.
        /// </summary>
        /// <param name="sender">The source object that raised the event, typically the UI element whose toggle state changed.</param>
        /// <param name="e">An object containing the event data, including the new toggle state.</param>
        private void RemoveQuery_Toggled(object sender, ToggledEventArgs e)
        {
            this.options.RemoveQueryParameters = e.Value;
            this.DetermineAllowedParamsVisibility();
        }

        /// <summary>
        /// Handles the toggling of the host replacement option by updating the domain replacement setting and adjusting
        /// host preference visibility.
        /// </summary>
        /// <param name="sender">The source of the event, typically the control that triggered the toggle action.</param>
        /// <param name="e">The event data containing the new toggle state.</param>
        private void HostReplacement_Toggled(object sender, ToggledEventArgs e)
        {
            this.options.ReplaceDomains = e.Value;
            this.DetermineHostPreferenceVisibility();
        }

        /// <summary>
        /// Handles the toggling of the 'Include www in domain matching' option when the associated UI element is
        /// toggled.
        /// </summary>
        /// <param name="sender">The source of the event, typically the UI element that was toggled.</param>
        /// <param name="e">The event data containing the new toggle state.</param>
        private void WwwMatch_Toggled(object sender, ToggledEventArgs e)
        {
            this.options.IncludeWwwInDomainMatching = e.Value;
        }

        /// <summary>
        /// Updates the visibility and fade animation of host preference UI elements based on the current domain
        /// replacement option.
        /// </summary>
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

        /// <summary>
        /// Updates the visibility of the allowed parameters list based on the current options.
        /// </summary>
        private async void DetermineAllowedParamsVisibility()
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

        /// <summary>
        /// Handles the About button click event by displaying information about LinkLaunderer and offering to copy the
        /// project URL to the clipboard.
        /// </summary>
        /// <param name="sender">The source of the event, typically the About button that was clicked.</param>
        /// <param name="e">An EventArgs instance containing event data associated with the click event.</param>
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

        /// <summary>
        /// Handles the print button click event by displaying the current settings in a dialog and optionally copying
        /// them to the clipboard.
        /// </summary>
        /// <param name="sender">The source of the event, typically the print button that was clicked.</param>
        /// <param name="e">An EventArgs instance containing event data associated with the click event.</param>
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

        /// <summary>
        /// Handles the reset button click event by prompting the user to confirm resetting all settings to their
        /// application defaults.
        /// </summary>
        /// <remarks>If the user confirms the reset, all settings are restored to their default values and
        /// saved to preferences. This action cannot be undone.</remarks>
        /// <param name="sender">The source of the event, typically the reset button control.</param>
        /// <param name="e">An EventArgs instance containing event data.</param>
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

        /// <summary>
        /// Handles the click event for deleting a host replacement entry from the domain replacements list.
        /// </summary>
        /// <remarks>After removing the host replacement, the method refreshes the host replacement list
        /// to reflect the changes. This event handler assumes the sender's BindingContext is a KeyValuePair containing
        /// the relevant key.</remarks>
        /// <param name="sender">The source of the event, expected to be a Button whose BindingContext contains the key of the host
        /// replacement to remove.</param>
        /// <param name="e">An EventArgs instance that contains the event data.</param>
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

        /// <summary>
        /// Handles the click event to add or update a host name replacement entry. Prompts the user for a host name and
        /// its replacement, and updates the replacement list accordingly.
        /// </summary>
        /// <param name="sender">The source of the event, typically the UI element that was clicked.</param>
        /// <param name="e">An EventArgs instance containing event data.</param>
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

            if (this.options.DomainReplacements.ContainsKey(host)
                && !await this.DisplayAlertAsync(
                    title: "Host Already Exists",
                    message: "This hostname already exists - replace entry?",
                    accept: "Replace",
                    cancel: "Cancel"))
            {
                return;
            }

            this.options.DomainReplacements[host] = replacement;

            // This is a hack due to not doing proper binding with an observable collection
            this.HostReplacementList.ItemsSource = null;
            this.HostReplacementList.ItemsSource = this.options.DomainReplacements;
        }

        /// <summary>
        /// Handles the event triggered when the user requests to add a new allowed query parameter.
        /// </summary>
        /// <remarks>Prompts the user to enter a query parameter name and adds it to the allowed
        /// parameters list if it is not already present and is not empty or whitespace.</remarks>
        /// <param name="sender">The source of the event, typically the UI element that was interacted with.</param>
        /// <param name="e">An EventArgs instance containing event data.</param>
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

        /// <summary>
        /// Handles the click event for deleting a parameter from the allowed parameters list.
        /// </summary>
        /// <param name="sender">The source of the event, expected to be a Button whose BindingContext contains the parameter key to remove.</param>
        /// <param name="e">An EventArgs instance containing event data.</param>
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

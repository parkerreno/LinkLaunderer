namespace LinkLaunderer.Controls;

/// <summary>
/// A custom control representing a settings item with a label, description, and toggle switch.
/// </summary>
public partial class SettingsItem : ContentView
{
    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(SettingsItem), string.Empty);

    public static readonly BindableProperty DescriptionTextProperty =
        BindableProperty.Create(nameof(DescriptionText), typeof(string), typeof(SettingsItem), string.Empty);

    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(SettingsItem), false, BindingMode.TwoWay);

    public event EventHandler<ToggledEventArgs>? Toggled;

    /// <summary>
    /// Gets or sets the text displayed by the label control.
    /// </summary>
    public string LabelText
    {
        get => (string)this.GetValue(LabelTextProperty);
        set => this.SetValue(LabelTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the descriptive text associated with the control.
    /// </summary>
    public string DescriptionText
    {
        get => (string)this.GetValue(DescriptionTextProperty);
        set => this.SetValue(DescriptionTextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is in the toggled state.
    /// </summary>
    public bool IsToggled
    {
        get => (bool)this.GetValue(IsToggledProperty);
        set => this.SetValue(IsToggledProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the SettingsItem class and sets up its user interface components.
    /// </summary>
    public SettingsItem()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Handles the toggled event for a switch control and raises the corresponding Toggled event.
    /// </summary>
    /// <param name="sender">The source of the event, typically the switch control that was toggled.</param>
    /// <param name="e">The event data containing information about the toggle state.</param>
    private void OnSwitchToggled(object? sender, ToggledEventArgs e)
    {
        Toggled?.Invoke(this, e);
    }
}
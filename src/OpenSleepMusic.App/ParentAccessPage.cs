using OpenSleepMusic.App.Localization;

namespace OpenSleepMusic.App;

internal enum ParentAccessPurpose { Enable, Unlock, ChangePin, Collections }

internal sealed class ParentAccessPage : ContentPage
{
    private readonly Entry _pin;
    private readonly Entry _confirmation;
    private readonly Label _error;
    private readonly Button _submit;
    private readonly ParentAccessPurpose _purpose;
    private bool _busy;
    private readonly Dictionary<string, CheckBox> _collections = [];

    public ParentAccessPage(ParentAccessPurpose purpose, IReadOnlyList<ChildCollectionOption>? collections = null)
    {
        _purpose = purpose;
        BackgroundColor = Color.FromArgb("#0B1020");
        Title = AppText.Get(purpose == ParentAccessPurpose.Enable ? "ChildSafetySetup"
            : purpose == ParentAccessPurpose.Unlock ? "ParentArea"
            : purpose == ParentAccessPurpose.Collections ? "ChooseChildCollections" : "ChangeParentPin");
        var heading = new Label { Text = Title, FontSize = 24, FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
        var help = new Label
        {
            Text = AppText.Get(purpose == ParentAccessPurpose.Enable ? "ChildSafetySetupHelp"
                : purpose == ParentAccessPurpose.Unlock ? "ParentUnlockHelp"
                : purpose == ParentAccessPurpose.Collections ? "ChildCollectionsHelp" : "PinFormat"),
            TextColor = Color.FromArgb("#D7D3E5"), FontSize = 16
        };
        _pin = CreateEntry("ParentPinEntry", AppText.Get(purpose == ParentAccessPurpose.Unlock ? "ParentPin" : "NewParentPin"));
        _confirmation = CreateEntry("ParentPinConfirmation", AppText.Get("ConfirmParentPin"));
        _pin.IsVisible = purpose != ParentAccessPurpose.Collections;
        _confirmation.IsVisible = purpose is ParentAccessPurpose.Enable or ParentAccessPurpose.ChangePin;
        _error = new Label { TextColor = Color.FromArgb("#FFB0C8"), AutomationId = "ParentPinError" };
        _submit = new Button
        {
            Text = AppText.Get(purpose == ParentAccessPurpose.Enable ? "EnableChildSafety"
                : purpose == ParentAccessPurpose.Unlock ? "UnlockParentArea"
                : purpose == ParentAccessPurpose.Collections ? "SaveChildCollections" : "SaveParentPin"),
            AutomationId = "SubmitParentPin", BackgroundColor = Color.FromArgb("#7367F0"), TextColor = Colors.White
        };
        _submit.Clicked += OnSubmit;
        var close = new Button { Text = AppText.Get("Cancel"), AutomationId = "CancelParentPin" };
        close.Clicked += async (_, _) => { if (!_busy) await Navigation.PopModalAsync(); };
        var content = new VerticalStackLayout { Spacing = 16, Padding = 24, MaximumWidthRequest = 620, HorizontalOptions = LayoutOptions.Fill };
        content.Add(heading);
        content.Add(help);
        if (purpose is ParentAccessPurpose.Enable or ParentAccessPurpose.Collections)
        {
            if (purpose == ParentAccessPurpose.Enable)
                content.Add(new Label { Text = AppText.Get("ChildCollectionsHelp"), TextColor = Color.FromArgb("#D7D3E5") });
            foreach (var collection in collections ?? [])
            {
                var checkbox = new CheckBox
                {
                    IsChecked = ChildSafety.Control.IsCollectionSelected(collection.Id),
                    AutomationId = $"ChildCollection_{collection.Id}", Color = Color.FromArgb("#AFA7FF")
                };
                SemanticProperties.SetDescription(checkbox, collection.Name);
                _collections.Add(collection.Id, checkbox);
                var label = new Label
                {
                    Text = collection.Name + " · " + AppText.Format("ChildDownloadedTracks", collection.DownloadedCount),
                    TextColor = Colors.White, VerticalOptions = LayoutOptions.Center
                };
                var toggle = new TapGestureRecognizer();
                toggle.Tapped += (_, _) => checkbox.IsChecked = !checkbox.IsChecked;
                label.GestureRecognizers.Add(toggle);
                var row = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) }, ColumnSpacing = 8 };
                row.Add(checkbox, 0, 0);
                row.Add(label, 1, 0);
                content.Add(row);
            }
        }
        if (purpose == ParentAccessPurpose.Enable)
            content.Add(new Label { Text = AppText.Get("PinRecoveryHelp"), TextColor = Color.FromArgb("#B8B5C8") });
        content.Add(_pin);
        content.Add(_confirmation);
        content.Add(_error);
        content.Add(_submit);
        content.Add(close);
        Content = new ScrollView { Content = content };
    }

    private static Entry CreateEntry(string id, string placeholder) => new()
    {
        AutomationId = id, Placeholder = placeholder, IsPassword = true, Keyboard = Keyboard.Numeric,
        MaxLength = 12, TextColor = Colors.White, PlaceholderColor = Color.FromArgb("#B8B5C8"),
        BackgroundColor = Color.FromArgb("#1A2035")
    };

    private async void OnSubmit(object? sender, EventArgs e)
    {
        if (_busy) return;
        if ((_purpose is ParentAccessPurpose.Enable or ParentAccessPurpose.ChangePin) && _pin.Text != _confirmation.Text)
        {
            _error.Text = AppText.Get("PinMismatch");
            return;
        }
        _busy = true;
        _submit.IsEnabled = false;
        try
        {
            var error = await ChildSafety.SubmitAsync(_purpose, _pin.Text ?? "",
                _collections.Where(item => item.Value.IsChecked).Select(item => item.Key).ToArray());
            _pin.Text = _confirmation.Text = "";
            if (error is null) await Navigation.PopModalAsync();
            else _error.Text = error;
        }
        finally
        {
            _busy = false;
            _submit.IsEnabled = true;
        }
    }

    protected override void OnDisappearing()
    {
        _pin.Text = _confirmation.Text = "";
        base.OnDisappearing();
    }
}

namespace OpenSleepMusic.App.Localization;

/// <summary>Resolves text when a page is created in the selected app language.</summary>
[ContentProperty(nameof(Key))]
public sealed class TextExtension : IMarkupExtension<string>
{
    public string Key { get; set; } = string.Empty;

    public string ProvideValue(IServiceProvider serviceProvider) => AppText.Get(Key);

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}

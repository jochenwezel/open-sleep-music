using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OpenSleepMusic.App.Localization;

namespace OpenSleepMusic.Core.Tests;

public sealed class AppLanguageTests : IDisposable
{
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _uiCulture = CultureInfo.CurrentUICulture;
    private readonly CultureInfo? _defaultCulture = CultureInfo.DefaultThreadCurrentCulture;
    private readonly CultureInfo? _defaultUiCulture = CultureInfo.DefaultThreadCurrentUICulture;
    private readonly AppLanguage _language = AppText.SelectedLanguage;

    public void Dispose()
    {
        AppText.Apply(_language);
        CultureInfo.CurrentCulture = _culture;
        CultureInfo.CurrentUICulture = _uiCulture;
        CultureInfo.DefaultThreadCurrentCulture = _defaultCulture;
        CultureInfo.DefaultThreadCurrentUICulture = _defaultUiCulture;
    }

    [Fact]
    public void LullabyCollectionHasEnglishNameAndDescription()
    {
        try
        {
            AppText.Apply(AppLanguage.English);

            Assert.Equal("Lullabies for little ones", AppText.WorldName("lullabies", "Schlaflieder für Kleine"));
            Assert.Equal(
                "Gentle piano, music-box and instrumental pieces without vocals.",
                AppText.WorldDescription("lullabies", "Sanfte Klavier-, Spieluhr- und Instrumentalstücke ohne Gesang."));
        }
        finally
        {
            AppText.Apply(AppLanguage.System);
        }
    }

    [Theory]
    [InlineData("de-DE", "de")]
    [InlineData("de-AT", "de")]
    [InlineData("en-GB", "en")]
    [InlineData("fr-FR", "en")]
    public void LanguageSelectionRespectsSystemAndExplicitOverrides(string system, string expected)
    {
        var culture = CultureInfo.GetCultureInfo(system);
        Assert.Equal(expected, AppText.ResolveCulture(AppLanguage.System, culture).Name);
        Assert.Equal("de", AppText.ResolveCulture(AppLanguage.German, culture).Name);
        Assert.Equal("en", AppText.ResolveCulture(AppLanguage.English, culture).Name);
    }

    [Fact]
    public void ReturningToSystemLanguageDoesNotUsePreviousAppOverride()
    {
        AppText.Apply(AppLanguage.System);
        var systemLanguage = CultureInfo.CurrentUICulture.Name;
        AppText.Apply(AppText.IsGerman ? AppLanguage.English : AppLanguage.German);
        Assert.NotEqual(systemLanguage, CultureInfo.CurrentUICulture.Name);
        AppText.Apply(AppLanguage.System);
        Assert.Equal(systemLanguage, CultureInfo.CurrentUICulture.Name);
        Assert.Equal(AppLanguage.System, AppText.SelectedLanguage);
        Assert.Equal(systemLanguage, CultureInfo.DefaultThreadCurrentUICulture?.Name);
    }

    [Fact]
    public void BothLanguagesCoverEveryResourceAndFormattingPlaceholder()
    {
        Assert.Equal(AppText.Keys(AppLanguage.German).Order(), AppText.Keys(AppLanguage.English).Order());
        foreach (var key in AppText.Keys(AppLanguage.German))
        {
            AppText.Apply(AppLanguage.German);
            var german = AppText.Get(key);
            AppText.Apply(AppLanguage.English);
            var english = AppText.Get(key);
            Assert.False(string.IsNullOrWhiteSpace(german), key);
            Assert.False(string.IsNullOrWhiteSpace(english), key);
            Assert.DoesNotContain('\uFFFD', german + english);
            Assert.Equal(Placeholders(german), Placeholders(english));
            _ = string.Format(german, "a", "b", "c", "d");
            _ = string.Format(english, "a", "b", "c", "d");
        }
    }

    [Fact]
    public void XamlProseAndAccessibleLabelsUseExistingTranslationKeys()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames().Where(name => name.StartsWith("Localization.")).ToArray();
        Assert.True(resources.Length >= 8);
        foreach (var resource in resources)
        {
            using var stream = assembly.GetManifestResourceStream(resource)!;
            var document = XDocument.Load(stream);
            foreach (var attribute in document.Descendants().Attributes().Where(attribute =>
                attribute.Name.LocalName is "Text" or "Title" or "SemanticProperties.Description" or "Placeholder"))
            {
                var value = attribute.Value;
                if (value.StartsWith("{loc:Text "))
                {
                    var key = value[10..^1];
                    Assert.Contains(key, AppText.Keys(AppLanguage.German));
                    Assert.Contains(key, AppText.Keys(AppLanguage.English));
                }
                else if (!value.StartsWith('{') && value.Any(char.IsLetter))
                {
                    Assert.True(value is "Open Sleep Music" or "open sleep music" or "Zzz",
                        $"Unlocalized text in {resource}: {attribute}");
                }
            }
        }
    }

    [Theory]
    [InlineData(-1, "noch 0 Sek.", "0 sec remaining")]
    [InlineData(0.1, "noch 1 Sek.", "1 sec remaining")]
    [InlineData(60, "noch 60 Sek.", "60 sec remaining")]
    [InlineData(61, "noch 2 Min.", "2 min remaining")]
    public void TimerCountdownIsLocalizedAndRoundsUp(double seconds, string german, string english)
    {
        AppText.Apply(AppLanguage.German);
        Assert.Equal(german, AppText.TimerRemaining(TimeSpan.FromSeconds(seconds)));
        AppText.Apply(AppLanguage.English);
        Assert.Equal(english, AppText.TimerRemaining(TimeSpan.FromSeconds(seconds)));
    }

    [Fact]
    public void EnglishFeedbackSummaryRetainsScopeAndUserControlledSending()
    {
        AppText.Apply(AppLanguage.English);
        var summary = AppText.Format("FeedbackSummary", "support@example.test", 2, 1, AppText.Get("WithComment"));
        Assert.Contains("support@example.test\n\n", summary);
        Assert.Contains("2 favorite(s), 1 block(s)", summary);
        Assert.Contains("as well as your comment.", summary);
        Assert.Contains("No device identifier, playback history or file paths", summary);
        Assert.Contains("You review and send the message yourself", summary);
    }

    private static string[] Placeholders(string value) =>
        Regex.Matches(value, @"\{\d+[^}]*\}").Select(match => match.Value).Distinct().Order().ToArray();

    [Theory]
    [InlineData("guitar", "Gitarre", "Guitar")]
    [InlineData("viola", "Bratsche", "Viola")]
    [InlineData("double-bass", "Kontrabass", "Double bass")]
    [InlineData("bassoon", "Fagott", "Bassoon")]
    [InlineData("nature", "Naturklänge", "Nature sounds")]
    [InlineData("birds", "Vögel", "Birds")]
    [InlineData("future-tag", "future-tag", "future-tag")]
    public void InstrumentLabelsAreLocalizedAndUnknownTagsArePreserved(string tag, string german, string english)
    {
        AppText.Apply(AppLanguage.German);
        Assert.Equal(german, AppText.InstrumentName(tag));
        AppText.Apply(AppLanguage.English);
        Assert.Equal(english, AppText.InstrumentName(tag));
    }
}

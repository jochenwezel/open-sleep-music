using System.Text.RegularExpressions;
using OpenSleepMusic.App.Localization;

namespace OpenSleepMusic.App;

/// <summary>The published German notice stays readable offline, including in locked child mode.</summary>
internal sealed partial class PrivacyNoticePage : ContentPage
{
    private static readonly Uri PublishedNotice = new("https://www.compumaster.de/datenschutzhinweis/apps/open-sleep-music/#de");
    private readonly Label _status = new() { TextColor = Color.FromArgb("#FFB0C8") };

    public PrivacyNoticePage(string markdown)
    {
        Title = AppText.Get("PrivacyNotice");
        BackgroundColor = Color.FromArgb("#0B1020");
        var close = new Button { Text = AppText.Get("Close"), AutomationId = "ClosePrivacyNotice" };
        close.Clicked += async (_, _) => await Navigation.PopModalAsync();
        var heading = new Label
        {
            Text = AppText.Get("PrivacyNotice"), FontSize = 24,
            FontAttributes = FontAttributes.Bold, TextColor = Colors.White
        };
        var body = new VerticalStackLayout { Spacing = 16, MaximumWidthRequest = 760 };
        body.Add(new Label { Text = AppText.Get("PrivacyNoticeLanguage"), TextColor = Color.FromArgb("#B8B5C8") });
        foreach (var block in markdown.Replace("\r\n", "\n").Split("\n\n", StringSplitOptions.RemoveEmptyEntries))
        {
            var text = string.Join(" ", block.Split('\n').Select(line => line.Trim()));
            var isHeading = text.StartsWith('#');
            body.Add(new Label
            {
                Text = isHeading ? text.TrimStart('#', ' ') : ReadableLinks().Replace(text, "${label} (${url})"),
                FontSize = isHeading ? 20 : 16,
                FontAttributes = isHeading ? FontAttributes.Bold : FontAttributes.None,
                TextColor = Color.FromArgb("#F7F3FF"), LineBreakMode = LineBreakMode.WordWrap
            });
        }
        var online = new Button { Text = AppText.Get("PrivacyNoticeOnline"), AutomationId = "OpenOnlinePrivacyNotice" };
        online.Clicked += async (_, _) =>
        {
            if (!await ChildSafety.RequireParentAsync(this)) return;
            try
            {
                if (!await Launcher.Default.OpenAsync(PublishedNotice)) _status.Text = PublishedNotice.AbsoluteUri;
            }
            catch { _status.Text = PublishedNotice.AbsoluteUri; }
        };
        body.Add(online);
        body.Add(_status);
        var layout = new Grid
        {
            Padding = 20, RowSpacing = 14,
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) }
        };
        layout.Add(heading, 0, 0);
        layout.Add(new ScrollView { Content = body }, 0, 1);
        layout.Add(close, 0, 2);
        Content = layout;
    }

    [GeneratedRegex(@"\[(?<label>[^\]]+)\]\((?<url>https://[^)]+)\)")]
    private static partial Regex ReadableLinks();
}

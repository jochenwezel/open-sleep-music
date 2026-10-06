using OpenSleepMusic.App.Localization;
using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.App;

internal sealed class PreselectionPage : ContentPage
{
    public PreselectionPage(AppStateStore stateStore)
    {
        Title = AppText.Pick("Vorauswahl", "Preselection");
        BackgroundColor = Color.FromArgb("#0B1020");
        var close = new Button { Text = AppText.Get("Close") };
        close.Clicked += async (_, _) => await Navigation.PopModalAsync();
        var introduction = new Label
        {
            Text = AppText.Pick(
                $"{BuiltInPreselection.Candidates.Count(candidate => candidate.DownloadUri is null)} weitere Hörreferenzen ohne geprüfte Downloadquelle. Die Aufnahme öffnet sich auf ihrer Originalseite im Browser. Lizenz und Schlaf-Eignung sind noch ungeprüft. Herunterladbare Kandidaten findest du direkt in der Sammlung Vorauswahl.",
                $"{BuiltInPreselection.Candidates.Count(candidate => candidate.DownloadUri is null)} additional listening references without checked download delivery. Recordings open on their original pages in your browser. Recording rights and sleep suitability are unreviewed. Downloadable candidates are in the Preselection collection."),
            TextColor = Colors.White
        };
        var list = new CollectionView
        {
            ItemsSource = BuiltInPreselection.Candidates.Where(candidate => candidate.DownloadUri is null).ToArray(),
            SelectionMode = SelectionMode.None,
            ItemTemplate = new DataTemplate(() =>
            {
                var title = new Label { FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
                title.SetBinding(Label.TextProperty, nameof(ReviewCandidate.Title));
                var metadata = new Label { TextColor = Color.FromArgb("#B8B5C8"), FontSize = 12 };
                var open = new Button { Text = AppText.Pick("Auf Quellseite probehören ↗", "Listen on source page ↗") };
                var favorite = new Button();
                var block = new Button();
                var row = new VerticalStackLayout { Spacing = 8, Padding = 12 };
                void Refresh()
                {
                    if (row.BindingContext is not ReviewCandidate candidate) return;
                    favorite.Text = stateStore.IsFavorite("preselection", candidate.Id) ? "★" : "☆";
                    block.Text = stateStore.IsBlocked("preselection", candidate.Id) ? AppText.Get("Unblock") : AppText.Get("Block");
                    metadata.Text = string.Join(", ", candidate.Instrumentation.Select(InstrumentName)) + " · " +
                        (candidate.LicenseReviewStatus == "verified" ? AppText.Pick("Lizenz geprüft", "License reviewed") :
                        candidate.LicenseReviewStatus == "rejected" ? AppText.Pick("Lizenz ausgeschlossen", "License rejected") :
                        AppText.Pick("Lizenz ungeprüft", "License unreviewed"));
                    if (candidate.IntendedWorldId == "lullabies")
                        metadata.Text += " · " + AppText.Pick("Für die Kleinen", "For little ones") + " · " +
                            (candidate.SelectionKind == "modern-lullaby" ? AppText.Pick("Modernes Wiegenlied", "Modern lullaby") :
                            candidate.SelectionKind == "gentle-arrangement" ? AppText.Pick("Sanfte Bearbeitung", "Gentle arrangement") :
                            AppText.Pick("Traditionelles Wiegenlied", "Traditional lullaby"));
                }
                row.BindingContextChanged += (_, _) => Refresh();
                open.Clicked += async (_, _) =>
                {
                    if (row.BindingContext is not ReviewCandidate candidate) return;
                    try
                    {
                        if (await Browser.Default.OpenAsync(candidate.SourcePageUri, BrowserLaunchMode.External)) return;
                    }
                    catch (Exception exception) { System.Diagnostics.Debug.WriteLine(exception); }
#if ANDROID
                    // Dispatch directly when Android package visibility hides an installed URL handler.
                    try
                    {
                        using var intent = new Android.Content.Intent(Android.Content.Intent.ActionView,
                            Android.Net.Uri.Parse(candidate.SourcePageUri.AbsoluteUri));
                        intent.AddFlags(Android.Content.ActivityFlags.NewTask);
                        Platform.AppContext.StartActivity(intent);
                        return;
                    }
                    catch (Exception exception) { System.Diagnostics.Debug.WriteLine(exception); }
#endif
                    var copy = await DisplayAlertAsync(AppText.Get("Source"),
                        AppText.Pick("Die Quellseite konnte nicht geöffnet werden. Link zum manuellen Öffnen kopieren?",
                            "The source page could not be opened. Copy the link to open it manually?"),
                        AppText.Pick("Link kopieren", "Copy link"), AppText.Get("Close"));
                    if (copy) await Clipboard.Default.SetTextAsync(candidate.SourcePageUri.AbsoluteUri);
                };
                favorite.Clicked += (_, _) =>
                {
                    if (row.BindingContext is not ReviewCandidate candidate) return;
                    stateStore.SetFavorite("preselection", candidate.Id, !stateStore.IsFavorite("preselection", candidate.Id));
                    Refresh();
                };
                block.Clicked += (_, _) =>
                {
                    if (row.BindingContext is not ReviewCandidate candidate) return;
                    stateStore.SetBlocked("preselection", candidate.Id, !stateStore.IsBlocked("preselection", candidate.Id));
                    Refresh();
                };
                var actions = new HorizontalStackLayout { Spacing = 8 };
                actions.Children.Add(favorite);
                actions.Children.Add(block);
                row.Children.Add(title);
                row.Children.Add(metadata);
                row.Children.Add(open);
                row.Children.Add(actions);
                return row;
            })
        };
        var layout = new Grid { Padding = 20, RowSpacing = 12, RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) } };
        layout.Add(introduction, 0, 0);
        layout.Add(list, 0, 1);
        layout.Add(close, 0, 2);
        Content = layout;
    }

    private static string InstrumentName(string tag) => tag switch
    {
        "piano" => AppText.Pick("Klavier", "Piano"),
        "music-box" => AppText.Pick("Spieluhr", "Music box"),
        "guitar" => AppText.Pick("Gitarre", "Guitar"),
        "harp" => AppText.Pick("Harfe", "Harp"),
        "pan-flute" => AppText.Pick("Panflöte", "Pan flute"),
        "flute" => AppText.Pick("Flöte", "Flute"),
        "strings" => AppText.Pick("Streicher", "Strings"),
        "orchestra" => AppText.Pick("Orchester", "Orchestra"),
        "saxophone" => AppText.Pick("Saxofon", "Saxophone"),
        _ => tag
    };
}

using OpenSleepMusic.App.Localization;

namespace OpenSleepMusic.Core.Tests;

public sealed class AppLanguageTests
{
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
}

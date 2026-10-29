using System.Xml.Linq;

namespace OpenSleepMusic.Core.Tests;

public sealed class AudioBackupPolicyTests
{
    [Fact]
    public void AndroidManifestReferencesRulesAndDoesNotRequestLibraryPermissions()
    {
        XNamespace android = "http://schemas.android.com/apk/res/android";
        var manifest = Read("AndroidManifest.xml");
        var app = manifest.Root!.Element("application")!;
        Assert.Equal("@xml/backup_rules", (string?)app.Attribute(android + "fullBackupContent"));
        Assert.Equal("@xml/data_extraction_rules", (string?)app.Attribute(android + "dataExtractionRules"));
        var permissions = manifest.Root.Elements("uses-permission")
            .Select(element => (string?)element.Attribute(android + "name"));
        Assert.DoesNotContain("android.permission.READ_MEDIA_AUDIO", permissions);
        Assert.DoesNotContain("android.permission.READ_EXTERNAL_STORAGE", permissions);
        Assert.DoesNotContain("android.permission.WRITE_EXTERNAL_STORAGE", permissions);
        Assert.DoesNotContain("android.permission.MANAGE_EXTERNAL_STORAGE", permissions);
    }

    [Fact]
    public void AudioIsExcludedFromLegacyBackupCloudBackupAndDeviceTransfer()
    {
        var legacy = Read("backup_rules.xml");
        var modern = Read("data_extraction_rules.xml");
        foreach (var scope in new[] { legacy.Root!, modern.Root!.Element("cloud-backup")!, modern.Root.Element("device-transfer")! })
        {
            Assert.Contains(scope.Elements("exclude"), element =>
                (string?)element.Attribute("domain") == "file" && (string?)element.Attribute("path") == "Audio/");
            Assert.Empty(scope.Elements("include")); // Other local settings retain their existing backup eligibility.
        }
    }

    private static XDocument Read(string name)
    {
        using var stream = typeof(AudioBackupPolicyTests).Assembly.GetManifestResourceStream($"AudioStorage.{name}");
        Assert.NotNull(stream);
        return XDocument.Load(stream);
    }
}

using System.Net;
using System.Text;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Tests;

public sealed class CatalogUpdateClientTests
{
    [Theory]
    [InlineData("https://example.test/source", true)]
    [InlineData("http://example.test/source", false)]
    public async Task ExternalReferencesLoadOnlyInPreviewWithSecureSource(string source, bool valid)
    {
        var root = TemporaryRoot();
        try
        {
            var json = System.Text.Json.Nodes.JsonNode.Parse(CatalogJson("pre-qualify"))!;
            var world = json["sleepWorlds"]![0]!;
            world["tracks"] = new System.Text.Json.Nodes.JsonArray();
            world["externalReferences"] = System.Text.Json.Nodes.JsonNode.Parse($$"""
                [{"id":"reference","title":"Audition","creator":"Artist",
                  "sourcePageUri":"{{source}}","instrumentation":["harp"],
                  "licenseReviewStatus":"unchecked","declaredLicense":"Unknown",
                  "reviewNotes":"Pending review"}]
                """);
            using var client = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.OK, json.ToJsonString())));
            var result = await new CatalogUpdateClient(client, CatalogUri).ResolveAsync(
                Snapshot("embedded", new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)), root);
            Assert.Equal(BuildChannel.IsPreview && valid ? "pre-qualify" : "embedded", Assert.Single(result.SleepWorlds).Id);
            if (BuildChannel.IsPreview && valid)
                Assert.Equal(source, Assert.Single(result.SleepWorlds[0].ExternalReferences!).SourcePageUri.AbsoluteUri);
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public async Task ValidRemoteCatalogReplacesEmbeddedCatalogAndSupportsOfflineCache()
    {
        var root = TemporaryRoot();
        try
        {
            using var online = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.OK, CatalogJson("remote"))));
            var embedded = Snapshot("embedded", new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var remote = await new CatalogUpdateClient(online, CatalogUri).ResolveAsync(embedded, root);
            Assert.Equal("remote", Assert.Single(remote.SleepWorlds).Id);
            Assert.False(File.Exists(Path.Combine(root, "media-catalog.json.part")));

            using var offline = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.NotFound, "missing")));
            var cached = await new CatalogUpdateClient(offline, CatalogUri).ResolveAsync(embedded, root);
            Assert.Equal("remote", Assert.Single(cached.SleepWorlds).Id);
        }
        finally { Directory.Delete(root, true); }
    }

    [Theory]
    [InlineData("<html>not json</html>")]
    [InlineData("{\"schemaVersion\":2,\"generatedAtUtc\":\"2026-01-01T00:00:00Z\",\"sleepWorlds\":[]}")]
    public async Task InvalidRemoteCatalogFallsBackToEmbeddedCatalog(string content)
    {
        var root = TemporaryRoot();
        try
        {
            using var client = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.OK, content)));
            var embedded = Snapshot("embedded", new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var result = await new CatalogUpdateClient(client, CatalogUri).ResolveAsync(embedded, root);
            Assert.Equal(embedded, result);
            Assert.False(File.Exists(Path.Combine(root, "media-catalog.json.part")));
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public async Task InvalidTrackUrlRejectsEntireRemoteCatalog()
    {
        var root = TemporaryRoot();
        try
        {
            var invalid = CatalogJson("remote").Replace("https://example.test/audio.mp3", "http://example.test/audio.mp3");
            using var client = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.OK, invalid)));
            var embedded = Snapshot("embedded", new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var result = await new CatalogUpdateClient(client, CatalogUri).ResolveAsync(embedded, root);
            Assert.Equal("embedded", Assert.Single(result.SleepWorlds).Id);
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public async Task UncheckedTrackOutsidePreviewPreselectionIsRejected()
    {
        var root = TemporaryRoot();
        try
        {
            var invalid = CatalogJson("remote").Replace(
                "\"playbackSpeed\": 1",
                "\"playbackSpeed\": 1, \"licenseReviewStatus\": \"unchecked\"");
            using var client = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.OK, invalid)));
            var embedded = Snapshot("embedded", new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var result = await new CatalogUpdateClient(client, CatalogUri).ResolveAsync(embedded, root);
            Assert.Equal("embedded", Assert.Single(result.SleepWorlds).Id);
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public async Task OlderCachedCatalogDoesNotOverrideNewerEmbeddedCatalog()
    {
        var root = TemporaryRoot();
        try
        {
            await File.WriteAllTextAsync(Path.Combine(root, "media-catalog.json"), CatalogJson("cached"));
            using var offline = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.NotFound, "missing")));
            var embedded = Snapshot("embedded", new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero));
            var result = await new CatalogUpdateClient(offline, CatalogUri).ResolveAsync(embedded, root);
            Assert.Equal("embedded", Assert.Single(result.SleepWorlds).Id);
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public async Task OlderRemoteCatalogDoesNotDowngradeNewerEmbeddedCatalog()
    {
        var root = TemporaryRoot();
        try
        {
            using var client = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.OK, CatalogJson("remote"))));
            var embedded = Snapshot("embedded", new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero));
            var result = await new CatalogUpdateClient(client, CatalogUri).ResolveAsync(embedded, root);
            Assert.Equal("embedded", Assert.Single(result.SleepWorlds).Id);
            Assert.False(File.Exists(Path.Combine(root, "media-catalog.json")));
        }
        finally { Directory.Delete(root, true); }
    }

    private static readonly Uri CatalogUri = new("https://example.test/media-catalog.json");

    private static string TemporaryRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), $"open-sleep-catalog-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        return root;
    }

    private static CatalogSnapshot Snapshot(string worldId, DateTimeOffset generatedAt) =>
        new(generatedAt, [new SleepWorld(worldId, worldId, "Description", "icon", [])]);

    private static string CatalogJson(string worldId) => $$"""
        {
          "schemaVersion": 1,
          "generatedAtUtc": "2026-01-01T00:00:00Z",
          "sleepWorlds": [{
            "id": "{{worldId}}",
            "name": "Remote world",
            "description": "Remote description",
            "icon": "moon",
            "tracks": [{
              "id": "remote-track",
              "title": "Remote track",
              "creator": "Test creator",
              "downloadUri": "https://example.test/audio.mp3",
              "sourcePageUri": "https://example.test/source",
              "license": "CC0",
              "licenseUri": "https://creativecommons.org/publicdomain/zero/1.0/",
              "fileName": "remote-track.mp3",
              "durationSeconds": 60,
              "volumeGain": 1,
              "playbackSpeed": 1
            }]
          }]
        }
        """;

    private static HttpResponseMessage Response(HttpStatusCode status, string content) =>
        new(status) { Content = new StringContent(content, Encoding.UTF8, "application/json") };

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(response(request));
    }
}

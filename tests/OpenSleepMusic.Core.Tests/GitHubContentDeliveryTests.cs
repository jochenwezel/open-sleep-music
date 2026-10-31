using System.Net;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Tests;

public sealed class GitHubContentDeliveryTests
{
    private const string Asset = "https://github.com/jochenwezel/open-sleep-music/releases/download/test/audio.mp3";

    [Theory]
    [InlineData(Asset, true)]
    [InlineData("https://raw.githubusercontent.com/jochenwezel/open-sleep-music/0123456789012345678901234567890123456789/music/audio.mp3", true)]
    [InlineData("https://raw.githubusercontent.com/jochenwezel/open-sleep-music/main/audio.mp3", false)]
    [InlineData("https://github.com/another/repo/releases/download/test/audio.mp3", false)]
    [InlineData("https://github.com.evil.test/jochenwezel/open-sleep-music/releases/download/test/audio.mp3", false)]
    [InlineData("https://archive.org/audio.mp3", false)]
    [InlineData("http://github.com/jochenwezel/open-sleep-music/releases/download/test/audio.mp3", false)]
    [InlineData("https://user@github.com/jochenwezel/open-sleep-music/releases/download/test/audio.mp3", false)]
    [InlineData("https://github.com:444/jochenwezel/open-sleep-music/releases/download/test/audio.mp3", false)]
    public void OnlyProjectAssetOrPinnedRawLocationsCanStartRequests(string value, bool allowed) =>
        Assert.Equal(allowed, GitHubContentDelivery.IsContentUri(new(value)));

    [Fact]
    public async Task ForeignInitialUrlNeverReachesTheTransport()
    {
        var calls = 0;
        using var client = new HttpClient(new Handler(_ => { calls++; return new(HttpStatusCode.OK); }));
        await Assert.ThrowsAsync<InvalidDataException>(() => GitHubContentDelivery.GetAsync(client, new("https://archive.org/audio.mp3")));
        Assert.Equal(0, calls);
    }

    [Theory]
    [InlineData("https://archive.org/audio.mp3")]
    [InlineData("http://release-assets.githubusercontent.com/audio.mp3")]
    [InlineData("https://release-assets.githubusercontent.com.evil.test/audio.mp3")]
    public async Task ForeignAndDowngradeRedirectsAreStoppedBeforeTheirRequest(string target)
    {
        var calls = 0;
        using var client = new HttpClient(new Handler(_ =>
        {
            calls++;
            var response = new HttpResponseMessage(HttpStatusCode.Redirect);
            response.Headers.Location = new(target);
            return response;
        }));
        await Assert.ThrowsAsync<InvalidDataException>(() => GitHubContentDelivery.GetAsync(client, new(Asset)));
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task GithubAssetRedirectRetainsNormalDelivery()
    {
        var requests = new List<string>();
        using var client = new HttpClient(new Handler(request =>
        {
            requests.Add(request.RequestUri!.Host);
            var response = new HttpResponseMessage(requests.Count == 1 ? HttpStatusCode.Redirect : HttpStatusCode.OK);
            if (requests.Count == 1) response.Headers.Location = new("https://release-assets.githubusercontent.com/asset?signature=test");
            return response;
        }));
        using var result = await GitHubContentDelivery.GetAsync(client, new(Asset));
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(new[] { "github.com", "release-assets.githubusercontent.com" }, requests);
    }

    [Fact]
    public async Task RedirectLoopHasAFiniteLimit()
    {
        var calls = 0;
        using var client = new HttpClient(new Handler(_ =>
        {
            calls++;
            var response = new HttpResponseMessage(HttpStatusCode.Redirect);
            response.Headers.Location = new(Asset);
            return response;
        }));
        await Assert.ThrowsAsync<InvalidDataException>(() => GitHubContentDelivery.GetAsync(client, new(Asset)));
        Assert.Equal(6, calls);
    }

    [Fact]
    public void ForeignCollectionSpecificMotifIsRejectedWithoutRestrictingAttributionLinks()
    {
        var track = new AudioTrack("test", "Test", "Creator", new(Asset), new("https://archive.org/source"),
            "CC0 1.0", new("https://creativecommons.org/publicdomain/zero/1.0/"), "test.mp3", 60,
            CollectionArtwork: new Dictionary<string, CollectionArtwork>
            {
                ["lullabies"] = new(new(Asset), "background.png", new('0', 64),
                    new("https://external.test/motif.png"), "motif.png", new('1', 64))
            });
        Assert.Throws<InvalidDataException>(() => GitHubContentDelivery.ValidateTrack(track));
        GitHubContentDelivery.ValidateTrack(track with { CollectionArtwork = null });
    }

    [Fact]
    public async Task RedirectsShareOneHeaderTimeoutBudget()
    {
        using var client = new HttpClient(new SlowRedirectHandler()) { Timeout = TimeSpan.FromMilliseconds(100) };
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => GitHubContentDelivery.GetAsync(client, new(Asset)));
    }

    private sealed class SlowRedirectHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            await Task.Delay(30, token);
            var response = new HttpResponseMessage(HttpStatusCode.Redirect);
            response.Headers.Location = new(Asset);
            return response;
        }
    }

    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) =>
            Task.FromResult(respond(request));
    }
}

using System.Net;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;
using OpenSleepMusic.Core.Privacy;

namespace OpenSleepMusic.Core.Tests;

public sealed class ParentalControlTests
{
    private const string Asset = "https://github.com/jochenwezel/open-sleep-music/releases/download/test/audio.mp3";
    private static readonly string Credential = ParentPin.Create("123456");

    [Fact]
    public void ChildModeShowsOnlyAllowedDownloadedCollectionsWhileParentsKeepFullAccess()
    {
        var control = new ParentalControl();
        control.SetAllowedCollections(["lullabies"]);
        Assert.True(control.IsCollectionVisible("piano", false));
        control.Enable();
        Assert.True(control.IsCollectionVisible("lullabies", true));
        Assert.False(control.IsCollectionVisible("lullabies", false));
        Assert.False(control.IsCollectionVisible("piano", true));
        Assert.False(control.IsCollectionVisible("new-catalog-collection", true));
        Assert.Throws<InvalidOperationException>(() => control.SetAllowedCollections(["piano"]));
        Assert.True(control.TryUnlock("123456", Credential));
        Assert.True(control.IsCollectionVisible("piano", false));
        control.SetAllowedCollections(["piano"]);
        control.Lock();
        Assert.False(control.IsCollectionAllowed("lullabies"));
        Assert.True(control.IsCollectionAllowed("piano"));
        var restarted = new ParentalControl(true, allowedCollections: ["piano"]);
        Assert.False(restarted.IsCollectionAllowed("lullabies"));
        Assert.True(restarted.IsCollectionAllowed("piano"));
    }

    [Fact]
    public void EmptyChildSelectionStaysEmptyAndDoesNotBecomeAnAllCollectionsFallback()
    {
        var control = new ParentalControl(true, allowedCollections: []);
        Assert.False(control.IsCollectionVisible("lullabies", true));
        Assert.False(control.IsCollectionAllowed("piano"));
    }

    [Fact]
    public void OptionalModeDefaultsToFullAccessAndRestartsLockedWhenEnabled()
    {
        var ordinary = new ParentalControl();
        Assert.False(ordinary.IsEnabled);
        Assert.False(ordinary.IsLocked);
        Assert.False(ordinary.OnlineToken().IsCancellationRequested);
        ordinary.Lock();
        Assert.False(ordinary.IsLocked);
        var restored = new ParentalControl(enabled: true);
        Assert.True(restored.IsLocked);
        Assert.Throws<HttpRequestException>(() => restored.OnlineToken());
        Assert.Throws<InvalidOperationException>(restored.Disable);
    }

    [Fact]
    public void PinUnlocksOnlyUntilRelockAndProtectionCanBeDisabledByParent()
    {
        var control = new ParentalControl();
        var initial = control.OnlineToken();
        control.Enable();
        Assert.True(initial.IsCancellationRequested);
        Assert.False(control.TryUnlock("654321", Credential));
        Assert.True(control.IsLocked);
        Assert.True(control.TryUnlock("123456", Credential));
        var parentSession = control.OnlineToken();
        Assert.False(parentSession.IsCancellationRequested);
        control.Lock();
        Assert.True(parentSession.IsCancellationRequested);
        Assert.True(control.TryUnlock("123456", Credential));
        Assert.False(control.OnlineToken().IsCancellationRequested);
        control.Disable();
        control.Lock();
        Assert.False(control.IsEnabled);
        Assert.False(control.IsLocked);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1.invalid.invalid")]
    [InlineData("2.invalid.invalid")]
    public void MissingOrCorruptCredentialCannotUnlock(string? credential)
    {
        var control = new ParentalControl(true);
        Assert.False(control.TryUnlock("123456", credential));
        Assert.True(control.IsLocked);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("1234567890123")]
    [InlineData("１２３４５６")]
    [InlineData("12a456")]
    public void PinMustHaveFourToTwelveAsciiDigits(string pin) =>
        Assert.Throws<ArgumentException>(() => ParentPin.Create(pin));

    [Theory]
    [InlineData("0000")]
    [InlineData("1234")]
    [InlineData("12345")]
    [InlineData("123456")]
    [InlineData("123456789012")]
    public void SupportedPinLengthsAndLeadingZerosRoundTripAndUnlock(string pin)
    {
        var credential = ParentPin.Create(pin);
        Assert.True(ParentPin.Verify(pin, credential));
        var control = new ParentalControl(enabled: true);
        Assert.True(control.TryUnlock(pin, credential));
        control.Lock();
        Assert.False(control.TryUnlock("9876", credential));
        Assert.True(control.IsLocked);
        Assert.True(control.TryUnlock(pin, credential));
    }

    [Fact]
    public void CredentialUsesIndependentSaltsAndDoesNotContainThePin()
    {
        var first = ParentPin.Create("985641");
        var second = ParentPin.Create("985641");
        Assert.NotEqual(first, second);
        Assert.DoesNotContain("985641", first);
        Assert.True(ParentPin.Verify("985641", first));
        Assert.False(ParentPin.Verify("985642", first));
    }

    [Fact]
    public void FiveFailedAttemptsRequireWaitingEvenWithTheCorrectPinAndAfterRestart()
    {
        var clock = new Clock();
        var control = new ParentalControl(true, clock);
        for (var i = 0; i < 5; i++) Assert.False(control.TryUnlock("654321", Credential));
        Assert.Equal(TimeSpan.FromSeconds(30), control.RetryIn);
        Assert.False(control.TryUnlock("123456", Credential));
        var restarted = new ParentalControl(true, clock, control.FailedAttempts, control.RetryAfter);
        Assert.False(restarted.TryUnlock("123456", Credential));
        clock.Now = clock.Now.AddSeconds(30);
        Assert.True(restarted.TryUnlock("123456", Credential));
        Assert.Equal(0, restarted.FailedAttempts);
    }

    [Fact]
    public async Task LockedModeBlocksAllRequestsBeforeTransportAndUnlockRestoresNormalDelivery()
    {
        var calls = 0;
        var control = new ParentalControl(true);
        using var client = new HttpClient(new ParentalControlHandler(control, new Handler((_, _) =>
        {
            calls++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("audio") });
        })));
        await Assert.ThrowsAsync<HttpRequestException>(() => GitHubContentDelivery.GetAsync(client, new(Asset)));
        Assert.Equal(0, calls);
        Assert.True(control.TryUnlock("123456", Credential));
        using var response = await GitHubContentDelivery.GetAsync(client, new(Asset));
        Assert.Equal("audio", await response.Content.ReadAsStringAsync());
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task EnablingProtectionCancelsARequestStillWaitingForHeaders()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var control = new ParentalControl();
        using var client = new HttpClient(new ParentalControlHandler(control, new Handler(async (_, token) =>
        {
            entered.SetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, token);
            return new(HttpStatusCode.OK);
        })));
        var pending = GitHubContentDelivery.GetAsync(client, new(Asset));
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        control.Enable();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending.WaitAsync(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task LockingAfterHeadersClosesTheResponseStreamAndAbortsAnOngoingBodyRead()
    {
        var body = new BlockingStream();
        var control = new ParentalControl();
        using var client = new HttpClient(new ParentalControlHandler(control, new Handler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(body) }))));
        using var response = await GitHubContentDelivery.GetAsync(client, new(Asset));
        await using var stream = await response.Content.ReadAsStreamAsync();
        var read = stream.ReadAsync(new byte[8]).AsTask();
        await body.Reading.Task.WaitAsync(TimeSpan.FromSeconds(5));
        control.Enable();
        Assert.True(body.Closed);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => read.WaitAsync(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task LockDuringAudioDownloadRemovesPartialFileAndSkipsOtherResourcesWithoutTransport()
    {
        var body = new BlockingStream();
        var calls = 0;
        var control = new ParentalControl();
        using var client = new HttpClient(new ParentalControlHandler(control, new Handler((_, _) =>
        {
            calls++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(body) });
        })));
        var first = BuiltInCatalog.SleepWorlds.SelectMany(world => world.Tracks).First() with
        {
            Id = "first", DownloadUri = new(Asset), FileName = "first.mp3", Sha1 = null
        };
        var second = first with { Id = "second", FileName = "second.mp3" };
        var world = new SleepWorld("test", "Test", "", "", [first, second]);
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        try
        {
            var pending = new SleepWorldDownloader(client).DownloadAsync(world, root);
            await body.Reading.Task.WaitAsync(TimeSpan.FromSeconds(5));
            control.Enable();
            var result = await pending.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(2, result.SkippedCount);
            Assert.Equal(0, result.AvailableCount);
            Assert.Equal(1, calls);
            Assert.Empty(Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories));
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Fact]
    public async Task LockedCatalogUsesEmbeddedFallbackWithoutAnyNetworkCall()
    {
        var calls = 0;
        var control = new ParentalControl(true);
        using var client = new HttpClient(new ParentalControlHandler(control, new Handler((_, _) =>
        {
            calls++;
            throw new InvalidOperationException("Must not reach transport.");
        })));
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        try
        {
            var embedded = new CatalogSnapshot(BuiltInCatalog.GeneratedAtUtc, BuiltInCatalog.SleepWorlds);
            var result = await new CatalogUpdateClient(client, new(Asset)).ResolveAsync(embedded, root);
            Assert.Same(embedded, result);
            Assert.Equal(0, calls);
            Assert.False(File.Exists(Path.Combine(root, "media-catalog.json.part")));
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Fact]
    public async Task LockedArtworkUsesValidatedCacheWithoutAnyNetworkCall()
    {
        var control = new ParentalControl(true);
        using var client = new HttpClient(new ParentalControlHandler(control, new Handler((_, _) =>
            throw new InvalidOperationException("Must not reach transport."))));
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 1];
            var path = Path.Combine(root, "cached.png");
            await File.WriteAllBytesAsync(path, png);
            var track = BuiltInCatalog.SleepWorlds.SelectMany(world => world.Tracks).First() with
            {
                ArtworkUri = new(Asset), ArtworkFileName = "cached.png",
                ArtworkSha256 = Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(png))
            };
            Assert.Equal(path, await new ArtworkCache(client).GetAsync(track, root));
            Assert.False(File.Exists(path + ".part"));
        }
        finally { Directory.Delete(root, true); }
    }

    private sealed class Clock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = new(2026, 10, 31, 12, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class Handler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => send(request, cancellationToken);
    }

    private sealed class BlockingStream : Stream
    {
        private readonly TaskCompletionSource _closed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource Reading { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public bool Closed { get; private set; }
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            Reading.TrySetResult();
            await _closed.Task.WaitAsync(cancellationToken);
            throw new ObjectDisposedException(nameof(BlockingStream));
        }
        protected override void Dispose(bool disposing) { Closed = true; _closed.TrySetResult(); base.Dispose(disposing); }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}

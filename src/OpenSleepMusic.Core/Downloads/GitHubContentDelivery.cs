using System.Net;
using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Downloads;

/// <summary>App content uses our GitHub assets; source/attribution links are metadata only.</summary>
public static class GitHubContentDelivery
{
    private const string RepositoryPath = "/jochenwezel/open-sleep-music/";

    public static bool IsContentUri(Uri? uri)
    {
        if (!IsSecure(uri) || uri!.Query.Length != 0) return false;
        if (uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase))
            return uri.AbsolutePath.StartsWith(RepositoryPath + "releases/download/", StringComparison.Ordinal)
                && uri.Segments.Length >= 7;
        if (!uri.Host.Equals("raw.githubusercontent.com", StringComparison.OrdinalIgnoreCase)
            || !uri.AbsolutePath.StartsWith(RepositoryPath, StringComparison.Ordinal)) return false;
        var revision = uri.Segments.ElementAtOrDefault(3)?.TrimEnd('/');
        return revision is { Length: 40 } && revision.All(Uri.IsHexDigit) && uri.Segments.Length > 4;
    }

    public static bool IsDeliveryUri(Uri? uri) => IsSecure(uri) &&
        (IsContentUri(uri) || uri!.Host.Equals("release-assets.githubusercontent.com", StringComparison.OrdinalIgnoreCase)
            || uri.Host.Equals("objects.githubusercontent.com", StringComparison.OrdinalIgnoreCase)
            || uri.Host.Equals("github-releases.githubusercontent.com", StringComparison.OrdinalIgnoreCase));

    private static bool IsSecure(Uri? uri) => uri is { IsAbsoluteUri: true, Scheme: "https", Port: 443 }
        && uri.UserInfo.Length == 0 && uri.Fragment.Length == 0;

    public static HttpClient CreateHttpClient() => new(new HttpClientHandler
    {
        AllowAutoRedirect = false,
        UseCookies = false
    });

    public static async Task<HttpResponseMessage> GetAsync(
        HttpClient client, Uri uri, CancellationToken cancellationToken = default)
    {
        if (!IsContentUri(uri)) throw new InvalidDataException("Content URL is outside the project's GitHub delivery.");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (client.Timeout != Timeout.InfiniteTimeSpan) timeout.CancelAfter(client.Timeout);
        var current = uri;
        for (var redirect = 0; redirect <= 5; redirect++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, current);
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
            if (response.RequestMessage?.RequestUri is { } actual && actual != current)
            {
                response.Dispose();
                throw new InvalidDataException("Content client must disable automatic redirects.");
            }
            if (response.StatusCode is not (HttpStatusCode.MovedPermanently or HttpStatusCode.Redirect
                or HttpStatusCode.SeeOther or HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect))
                return response;
            var location = response.Headers.Location;
            response.Dispose();
            if (location is null) throw new InvalidDataException("Content redirect has no destination.");
            var destination = new Uri(current, location);
            if (!IsDeliveryUri(destination))
                throw new InvalidDataException("Content redirect leaves GitHub delivery or HTTPS.");
            current = destination;
        }
        throw new InvalidDataException("Too many GitHub content redirects.");
    }

    public static void ValidateTrack(AudioTrack track)
    {
        if (!IsContentUri(track.DownloadUri)
            || new[] { track.ArtworkUri, track.SongMotifUri, track.FallbackMotifUri }
                .Any(uri => uri is not null && !IsContentUri(uri))
            || track.CollectionArtwork?.Values.Any(layer =>
                new[] { layer.ArtworkUri, layer.SongMotifUri, layer.FallbackMotifUri }
                    .Any(uri => uri is not null && !IsContentUri(uri))) == true)
            throw new InvalidDataException($"Track '{track.Id}' contains a non-GitHub content URL.");
    }
}

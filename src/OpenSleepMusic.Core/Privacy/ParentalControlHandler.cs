using System.Net;

namespace OpenSleepMusic.Core.Privacy;

/// <summary>Guards every network hop and cancels ongoing response bodies when child mode starts.</summary>
public sealed class ParentalControlHandler(ParentalControl control, HttpMessageHandler innerHandler)
    : DelegatingHandler(innerHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, control.OnlineToken());
        try
        {
            scope.Token.ThrowIfCancellationRequested();
            var response = await base.SendAsync(request, scope.Token);
            if (scope.IsCancellationRequested)
            {
                response.Dispose();
                scope.Token.ThrowIfCancellationRequested();
            }
            response.Content = new GuardedContent(response.Content, scope);
            return response;
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    private sealed class GuardedContent : HttpContent
    {
        private readonly HttpContent _inner;
        private readonly CancellationTokenSource _scope;
        private readonly CancellationTokenRegistration _registration;

        public GuardedContent(HttpContent inner, CancellationTokenSource scope)
        {
            _inner = inner;
            _scope = scope;
            foreach (var header in inner.Headers) Headers.TryAddWithoutValidation(header.Key, header.Value);
            // Dispose the response stream even after ResponseHeadersRead has returned.
            _registration = scope.Token.Register(inner.Dispose);
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            _inner.CopyToAsync(stream, _scope.Token);

        protected override Task<Stream> CreateContentReadStreamAsync() => ReadStreamAsync();
        protected override Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken) => ReadStreamAsync();

        private Task<Stream> ReadStreamAsync()
        {
            _scope.Token.ThrowIfCancellationRequested();
            return _inner.ReadAsStreamAsync(_scope.Token);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _inner.Headers.ContentLength ?? 0;
            return _inner.Headers.ContentLength.HasValue;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _registration.Dispose();
                _inner.Dispose();
                _scope.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using Blazored.LocalStorage;
using WebApp.Extensions;

namespace WebApp.Infrastructure;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly ISyncLocalStorageService _storageService;

    public AuthTokenHandler(ISyncLocalStorageService storageService)
    {
        _storageService = storageService;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_storageService != null)
        {
            var token = _storageService.GetToken();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
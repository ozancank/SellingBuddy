using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using WebApp.Extensions;

namespace WebApp.Utils;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationState _anonymous;

    public AuthStateProvider(ILocalStorageService localStorageService, HttpClient httpClient)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClient;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string apiToken = await _localStorageService.GetToken();
        if (string.IsNullOrEmpty(apiToken))
            return _anonymous;

        string userName = await _localStorageService.GetUserName();

        var cp = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.NameIdentifier, userName)
            }, "jwtAuthType"));

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", apiToken);

        return new AuthenticationState(cp);
    }

    public void NotifyUserLogin(string userName)
    {
        var cp = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.Name, userName)
            }, "jwtAuthType"
        ));

        var authState = Task.FromResult(new AuthenticationState(cp));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
    }
}
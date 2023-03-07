using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Application.Services.Interfaces;
using WebApp.Domain.Models.User;
using WebApp.Extensions;
using WebApp.Utils;

namespace WebApp.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly ISyncLocalStorageService _syncLocalStorageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public IdentityService(HttpClient httpClient, ISyncLocalStorageService syncLocalStorageService, AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _syncLocalStorageService = syncLocalStorageService;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public bool IsLoggedIn => !string.IsNullOrEmpty(GetUserToken());

    public string GetUserName()
    {
        return _syncLocalStorageService.GetUserName();
    }

    public string GetUserToken()
    {
        return _syncLocalStorageService.GetToken();
    }

    public async Task<bool> Login(string userName, string password)
    {
        var req = new UserLoginRequest(userName, password);

        var response = await _httpClient.PostGetResponseAsync<UserLoginResponse, UserLoginRequest>("auth", req);

        if (!string.IsNullOrEmpty(response.UserToken))
        {
            _syncLocalStorageService.SetToken(response.UserToken);
            _syncLocalStorageService.SetUserName(response.UserName);

            ((AuthStateProvider)_authenticationStateProvider).NotifyUserLogin(response.UserName);

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", response.UserToken);

            return true;
        }

        return false;
    }

    public void Logout()
    {
        _syncLocalStorageService.RemoveItem("token");
        _syncLocalStorageService.RemoveItem("username");

        ((AuthStateProvider)_authenticationStateProvider).NotifyUserLogout();

        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
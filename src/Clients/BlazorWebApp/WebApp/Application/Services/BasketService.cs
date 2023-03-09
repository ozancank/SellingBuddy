using WebApp.Application.Services.Dtos;
using WebApp.Application.Services.Interfaces;
using WebApp.Domain.Models.ViewModels;
using WebApp.Extensions;

namespace WebApp.Application.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _apiClient;
    private readonly IIdentityService _identityService;
    private readonly ILogger<BasketService> _logger;

    public BasketService(HttpClient apiClient, IIdentityService identityService, ILogger<BasketService> logger)
    {
        _apiClient = apiClient;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Basket> GetBasket()
    {
        var response = await _apiClient.GetResponseAsync<Basket>("basket/" + _identityService.GetUserName());
        return response ?? new Basket() { BuyerId = _identityService.GetUserName() };
    }

    public async Task<Basket> UpdateBasket(Basket basket)
    {
        var response = await _apiClient.PostGetResponseAsync<Basket, Basket>("basket/update", basket);
        return response;
    }

    public async Task AddItemToBasket(int productId)
    {
        var model = new
        {
            CatalogItemId = productId,
            Quantity = 1,
            BasketId = _identityService.GetUserName()
        };

        //await _apiClient.PostAsync("basket/items", model);
        await _apiClient.PostAsync("basket/items", null);
    }

    public Task Checkout(BasketDTO basket)
    {
        //await _apiClient.PostAsync("basket/items", basket);
        return _apiClient.PostAsync("basket/checkout", null);
    }
}
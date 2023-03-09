using WebApp.Application.Services.Interfaces;
using WebApp.Domain.Models;
using WebApp.Domain.Models.CatalogModels;
using WebApp.Extensions;

namespace WebApp.Application.Services;

public class CatalogService : ICatalogService
{
    private readonly HttpClient _apiClient;

    public CatalogService(HttpClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PaginatedItemsViewModel<CatalogItem>> GetCatalogItems()
    {
        var response = await _apiClient.GetResponseAsync<PaginatedItemsViewModel<CatalogItem>>("/catalog/items");
        return response;
    }
}
using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Domain.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniShoppingApp.Infrastructure.Configuration;

namespace MiniShoppingApp.Infrastructure.Repositories;

public class ProductRepository(
        HttpClient httpClient,
        ILogger<ProductRepository> logger,
        IOptions<ApiSettings> apiSettings) : IProductRepository
{
    public async Task<ICollection<Product>> GetProductsAsync()
    {
        try
        {
            var productApiUrl = apiSettings.Value.ProductApiUrl;
            logger.LogInformation($"Fetching products from {productApiUrl}");
            var response = await httpClient.GetStringAsync(productApiUrl);

            return JsonSerializer.Deserialize<List<Product>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Product>();
        }
        catch (Exception ex)
        {
            logger.LogError($"Error fetching products: {ex.Message}");

            return new List<Product>();
        }
    }
}
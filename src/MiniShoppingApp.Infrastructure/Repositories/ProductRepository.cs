using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Domain.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MiniShoppingApp.Infrastructure.Repositories;

public class ProductRepository(HttpClient httpClient, ILogger<ProductRepository> logger) : IProductRepository
{
    private const string ApiUrl = "https://fakestoreapi.com/products";

    public async Task<ICollection<Product>> GetProductsAsync()
    {
        try
        {
            var response = await httpClient.GetStringAsync(ApiUrl);
            return JsonSerializer.Deserialize<List<Product>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            logger.LogError($"Error fetching products: {ex.Message}");
            return new List<Product>();
        }
    }
}
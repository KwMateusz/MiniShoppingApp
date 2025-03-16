using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Domain.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MiniShoppingApp.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductRepository> _logger;
    private const string ApiUrl = "https://fakestoreapi.com/products";

    public ProductRepository(HttpClient httpClient, ILogger<ProductRepository> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ICollection<Product>> GetProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(ApiUrl);
            return JsonSerializer.Deserialize<List<Product>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching products: {ex.Message}");
            return new List<Product>();
        }
    }
}
using System.Text.Json;
using MiniShoppingApp.Models;

namespace MiniShoppingApp.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductService> _logger;
    private const string ApiUrl = "https://fakestoreapi.com/products";

    public ProductService(HttpClient httpClient, ILogger<ProductService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Product>> GetProductsAsync()
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
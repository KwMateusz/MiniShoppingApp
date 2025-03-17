using System.Text.Json;
using System.Net.Http.Json;
using FluentAssertions;

namespace MiniShoppingApp.Test.Integration;

public class CartControllerTests
{
    private readonly HttpClient _client;

    public CartControllerTests()
    {
        var testServer = new CustomTestServer(); // Use custom test server
        _client = testServer.Client;
    }

    [Fact]
    public async Task AddToCart_ShouldReturnSuccessAndIncreaseCartCount_WhenProductExists()
    {
        // Arrange
        var productId = 1;

        // Act
        var response = await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement?>();

        // Assert
        response.EnsureSuccessStatusCode();
        jsonResponse?.GetProperty("success").GetBoolean().Should().BeTrue();
        jsonResponse?.GetProperty("cartCount").GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task AddToCart_ShouldIncreaseQuantity_WhenSameProductAddedAgain()
    {
        // Arrange
        var productId = 1;

        // Act
        await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);
        var response = await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement?>();

        // Assert
        response.EnsureSuccessStatusCode();
        jsonResponse?.GetProperty("success").GetBoolean().Should().BeTrue();
        jsonResponse?.GetProperty("cartCount").GetInt32().Should().Be(2);
    }

    [Fact]
    public async Task AddToCart_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = 999; // Non-existent product

        // Act
        var response = await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement?>();

        // Assert
        response.EnsureSuccessStatusCode();
        jsonResponse?.GetProperty("success").GetBoolean().Should().BeFalse();
    }

    [Fact]
    public async Task RemoveFromCart_ShouldDecreaseQuantity_WhenMultipleExists()
    {
        // Arrange
        var productId = 1;
        await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);
        await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);

        // Act
        var response = await _client.PostAsync($"/Cart/RemoveFromCart?productId={productId}", null);
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement?>();

        // Assert
        response.EnsureSuccessStatusCode();
        jsonResponse?.GetProperty("success").GetBoolean().Should().BeTrue();
        jsonResponse?.GetProperty("cartCount").GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task RemoveFromCart_ShouldRemoveItem_WhenOnlyOneExists()
    {
        // Arrange
        var productId = 1;
        await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);

        // Act
        var response = await _client.PostAsync($"/Cart/RemoveFromCart?productId={productId}", null);
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement?>();

        // Assert
        response.EnsureSuccessStatusCode();
        jsonResponse?.GetProperty("success").GetBoolean().Should().BeTrue();
        jsonResponse?.GetProperty("cartCount").GetInt32().Should().Be(0);
    }

    [Fact]
    public async Task RemoveFromCart_ShouldReturnFalse_WhenItemNotInCart()
    {
        // Arrange
        var productId = 1; // Not added to cart

        // Act
        var response = await _client.PostAsync($"/Cart/RemoveFromCart?productId={productId}", null);
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement?>();

        // Assert
        response.EnsureSuccessStatusCode();
        jsonResponse?.GetProperty("success").GetBoolean().Should().BeFalse();
    }

    [Fact]
    public async Task GetCartCount_ShouldReturnCorrectTotalQuantity()
    {
        // Arrange
        var productId1 = 1;
        var productId2 = 2;

        await _client.PostAsync($"/Cart/AddToCart?productId={productId1}", null);
        await _client.PostAsync($"/Cart/AddToCart?productId={productId1}", null);
        await _client.PostAsync($"/Cart/AddToCart?productId={productId2}", null);

        // Act
        var response = await _client.GetAsync("/Cart/GetCartCount");
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement?>();

        // Assert
        response.EnsureSuccessStatusCode();
        jsonResponse?.GetProperty("cartCount").GetInt32().Should().Be(3);
    }

    [Fact]
    public async Task GetProductQuantity_ShouldReturnCorrectQuantity()
    {
        // Arrange
        var productId = 1;
        await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);
        await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);

        // Act
        var response = await _client.GetAsync($"/Cart/GetProductQuantity?productId={productId}");
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement?>();

        // Assert
        response.EnsureSuccessStatusCode();
        jsonResponse?.GetProperty("productQuantity").GetInt32().Should().Be(2);
    }
}
using System.Net.Http.Json;

namespace MiniShoppingApp.Test.Integration;

public class CartControllerTests2(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetCartCount_ShouldReturnZero_WhenCartIsEmpty()
    {
        // Act
        var response = await _client.GetAsync("/Cart/GetCartCount");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadFromJsonAsync<dynamic>();

        // Assert
        jsonResponse.cartCount.Should().Be(0);
    }

    [Fact]
    public async Task AddToCart_ShouldReturnSuccessAndIncreaseCartCount()
    {
        // Arrange
        var productId = 1;

        // Act
        var response = await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadFromJsonAsync<dynamic>();

        // Assert
        jsonResponse.success.Should().BeTrue();
        jsonResponse.cartCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task RemoveFromCart_ShouldReturnSuccess_WhenItemExists()
    {
        // Arrange
        var productId = 1;
        await _client.PostAsync($"/Cart/AddToCart?productId={productId}", null);

        // Act
        var response = await _client.PostAsync($"/Cart/RemoveFromCart?productId={productId}", null);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadFromJsonAsync<dynamic>();

        // Assert
        jsonResponse.success.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveFromCart_ShouldReturnFalse_WhenItemNotInCart()
    {
        // Arrange
        var productId = 99; // Non-existent product in cart

        // Act
        var response = await _client.PostAsync($"/Cart/RemoveFromCart?productId={productId}", null);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadFromJsonAsync<dynamic>();

        // Assert
        jsonResponse.success.Should().BeFalse();
    }

    [Fact]
    public async Task GetProductQuantity_ShouldReturnZero_WhenProductNotInCart()
    {
        // Arrange
        var productId = 2;

        // Act
        var response = await _client.GetAsync($"/Cart/GetProductQuantity?productId={productId}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadFromJsonAsync<dynamic>();

        // Assert
        jsonResponse.productQuantity.Should().Be(0);
    }
}
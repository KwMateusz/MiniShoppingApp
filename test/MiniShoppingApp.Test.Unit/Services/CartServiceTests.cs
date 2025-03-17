using System.Text.Json;
using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Infrastructure.Services;
using MiniShoppingApp.Domain.Models;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace MiniShoppingApp.Test.Unit.Services;

public class CartServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly Mock<ISession> _sessionMock = new();

    private readonly ICartService _sut;

    public CartServiceTests()
    {
        Dictionary<string, byte[]> sessionStorage = new(); // Simulated session storage

        List<Product> testProducts = new()
        {
            new Product { Id = 1, Title = "Product 1", Price = 10.0m },
            new Product { Id = 2, Title = "Product 2", Price = 15.0m },
            new Product { Id = 3, Title = "Product 3", Price = 20.0m }
        };

        // Mock Product Repository to return test products
        _productRepositoryMock.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(testProducts);

        // Configure session mock
        _sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
            .Callback<string, byte[]>((key, value) => sessionStorage[key] = value);

        _sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
            .Returns((string key, out byte[] value) =>
            {
                if (sessionStorage.TryGetValue(key, out var sessionData))
                {
                    value = sessionData;
                    return true;
                }

                value = JsonSerializer.SerializeToUtf8Bytes(new List<CartItem>()); // Return empty cart as JSON
                return true;
            });

        // Mock HttpContextAccessor to provide session
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(ctx => ctx.Session).Returns(_sessionMock.Object);
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);

        _sut = new CartService(_productRepositoryMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task AddToCart_ShouldAddNewItem_WhenProductExists()
    {
        // Act
        var success = await _sut.AddToCart(1);
        var cartItems = _sut.GetCartItems();

        // Assert
        success.Should().BeTrue();
        cartItems.Should().HaveCount(1);
        cartItems.First().Product.Id.Should().Be(1);
        cartItems.First().Quantity.Should().Be(1);
    }

    [Fact]
    public async Task AddToCart_ShouldIncreaseQuantity_WhenProductAlreadyInCart()
    {
        // Arrange
        await _sut.AddToCart(1); // First add

        // Act
        var success = await _sut.AddToCart(1); // Second add
        var cartItems = _sut.GetCartItems();

        // Assert
        success.Should().BeTrue();
        cartItems.Should().HaveCount(1);
        cartItems.First().Quantity.Should().Be(2);
    }

    [Fact]
    public async Task AddToCart_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Act
        var success = await _sut.AddToCart(999); // Non-existent product

        // Assert
        success.Should().BeFalse();
        _sut.GetCartItems().Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveFromCart_ShouldDecreaseQuantity_WhenMultipleExists()
    {
        // Arrange
        await _sut.AddToCart(1);
        await _sut.AddToCart(1);

        // Act
        var success = _sut.RemoveFromCart(1);
        var cartItems = _sut.GetCartItems();

        // Assert
        success.Should().BeTrue();
        cartItems.Should().HaveCount(1);
        cartItems.First().Quantity.Should().Be(1);
    }

    [Fact]
    public async Task RemoveFromCart_ShouldRemoveItem_WhenOnlyOneExists()
    {
        // Arrange
        await _sut.AddToCart(1);

        // Act
        var success = _sut.RemoveFromCart(1);
        var cartItems = _sut.GetCartItems();

        // Assert
        success.Should().BeTrue();
        cartItems.Should().BeEmpty();
    }

    [Fact]
    public void RemoveFromCart_ShouldReturnFalse_WhenItemNotInCart()
    {
        // Act
        var success = _sut.RemoveFromCart(1);

        // Assert
        success.Should().BeFalse();
    }

    [Fact]
    public async Task GetCartItemCount_ShouldReturnCorrectTotalQuantity()
    {
        // Arrange
        await _sut.AddToCart(1);
        await _sut.AddToCart(1);
        await _sut.AddToCart(2);

        // Act
        var totalItems = _sut.GetCartItemCount();

        // Assert
        totalItems.Should().Be(3);
    }

    [Fact]
    public async Task GetProductQuantity_ShouldReturnCorrectQuantity()
    {
        // Arrange
        await _sut.AddToCart(1);
        await _sut.AddToCart(1);
        await _sut.AddToCart(2);

        // Act
        var quantity1 = _sut.GetProductQuantity(1);
        var quantity2 = _sut.GetProductQuantity(2);
        var quantity3 = _sut.GetProductQuantity(999); // Non-existent product

        // Assert
        quantity1.Should().Be(2);
        quantity2.Should().Be(1);
        quantity3.Should().Be(0);
    }
}
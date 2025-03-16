using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Infrastructure.Services;
using MiniShoppingApp.Domain.Models;
using Moq;
using FluentAssertions;

namespace MiniShoppingApp.Test.Unit.Services;

public class CartServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;

    private ICartService _sut;

    public CartServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();

        _sut = new CartService(_productRepositoryMock.Object);

        List<Product> testProducts = new()
        {
            new Product { Id = 1, Title = "Product 1", Price = 10.0m },
            new Product { Id = 2, Title = "Product 2", Price = 15.0m },
            new Product { Id = 3, Title = "Product 3", Price = 20.0m },
        };

        _productRepositoryMock.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(testProducts);
    }

    [Fact]
    public async Task AddToCart_ShouldAddNewItem_WhenProductExists()
    {
        // Arrange
        _sut = new CartService(_productRepositoryMock.Object);

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
        _sut = new CartService(_productRepositoryMock.Object);
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
        // Arrange
        _sut = new CartService(_productRepositoryMock.Object);

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
        _sut = new CartService(_productRepositoryMock.Object);
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
        _sut = new CartService(_productRepositoryMock.Object);
        await _sut.AddToCart(1);

        // Act
        var success = _sut.RemoveFromCart(1);
        var cartItems = _sut.GetCartItems();

        // Assert
        success.Should().BeTrue();
        cartItems.Should().BeEmpty();
    }

    [Fact]
    public Task RemoveFromCart_ShouldReturnFalse_WhenItemNotInCart()
    {
        // Arrange
        _sut = new CartService(_productRepositoryMock.Object);

        // Act
        var success = _sut.RemoveFromCart(1);

        // Assert
        success.Should().BeFalse();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetCartItemCount_ShouldReturnCorrectTotalQuantity()
    {
        // Arrange
        _sut = new CartService(_productRepositoryMock.Object);
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
        _sut = new CartService(_productRepositoryMock.Object);
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
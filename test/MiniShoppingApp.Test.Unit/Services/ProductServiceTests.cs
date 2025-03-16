using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Application.Services;
using MiniShoppingApp.Domain.Models;
using Moq;
using FluentAssertions;

namespace MiniShoppingApp.Test.Unit.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly List<Product> _testProducts;

    private readonly IProductService _sut;

    public ProductServiceTests()
    {
        _sut = new ProductService(_productRepositoryMock.Object);

        _testProducts = new List<Product>
        {
            new() { Id = 1, Title = "Product 1", Price = 10.0m },
            new() { Id = 2, Title = "Product 2", Price = 15.0m },
            new() { Id = 3, Title = "Product 3", Price = 20.0m },
            new() { Id = 4, Title = "Product 4", Price = 25.0m },
            new() { Id = 5, Title = "Product 5", Price = 30.0m },
            new() { Id = 6, Title = "Product 6", Price = 35.0m }
        };
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnPagedProducts()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(_testProducts);

        // Act
        var (pagedProducts, totalPages) = await _sut.GetProductsAsync(page: 1, pageSize: 2);

        // Assert
        pagedProducts.Should().HaveCount(2);
        totalPages.Should().Be(3); // 6 products, pageSize 2 -> 3 pages
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnEmptyList_WhenNoProductsAvailable()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(new List<Product>());

        // Act
        var (pagedProducts, totalPages) = await _sut.GetProductsAsync(page: 1, pageSize: 2);

        // Assert
        pagedProducts.Should().BeEmpty();
        totalPages.Should().Be(0);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnCorrectTotalPages_WhenPageSizeIsLargerThanProductCount()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(_testProducts);

        // Act
        var (pagedProducts, totalPages) = await _sut.GetProductsAsync(page: 1, pageSize: 10);

        // Assert
        pagedProducts.Should().HaveCount(6);
        totalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnLastPage_WhenPageExceedsTotalPages()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(_testProducts);

        // Act
        var (pagedProducts, totalPages) = await _sut.GetProductsAsync(page: 5, pageSize: 2);

        // Assert
        pagedProducts.Should().BeEmpty(); // Page 5 does not exist (only 3 pages available)
        totalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnFirstPage_WhenPageNumberIsNegative()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(_testProducts);

        // Act
        var (pagedProducts, totalPages) = await _sut.GetProductsAsync(page: -1, pageSize: 2);

        // Assert
        pagedProducts.Should().HaveCount(2); // Should default to page 1
        totalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnDefaultPageSize_WhenPageSizeIsZeroOrNegative()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(_testProducts);

        // Act
        var (pagedProducts, totalPages) = await _sut.GetProductsAsync(page: 1, pageSize: -5);

        // Assert
        pagedProducts.Should().HaveCount(5); // Should default to 5 per page
        totalPages.Should().Be(2);
    }
}
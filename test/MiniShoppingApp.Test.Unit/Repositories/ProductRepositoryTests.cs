using MiniShoppingApp.Infrastructure.Repositories;
using MiniShoppingApp.Infrastructure.Configuration;
using MiniShoppingApp.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using MiniShoppingApp.Application.Interfaces;

namespace MiniShoppingApp.Test.Unit.Repositories;

public class ProductRepositoryTests
{
    private readonly Mock<ILogger<ProductRepository>> _loggerMock = new();
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
    private readonly IOptions<ApiSettings> _apiSettingsMock;
    private readonly List<Product> _testProducts;

    private readonly IProductRepository _sut;

    public ProductRepositoryTests()
    {
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://fakestoreapi.com/")
        };

        _apiSettingsMock = Options.Create(new ApiSettings
        {
            ProductApiUrl = "https://fakestoreapi.com/products"
        });

        _sut = new ProductRepository(httpClient, _loggerMock.Object, _apiSettingsMock);

        _testProducts = new List<Product>
        {
            new() { Id = 1, Title = "Product 1", Price = 10.0m },
            new() { Id = 2, Title = "Product 2", Price = 15.0m }
        };
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnProducts_WhenApiReturnsValidResponse()
    {
        // Arrange
        var jsonResponse = JsonSerializer.Serialize(_testProducts);
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _sut.GetProductsAsync();

        // Assert
        var resultList = result.ToList();
        resultList.Should().NotBeNull();
        resultList.Should().HaveCount(2);
        resultList[0].Id.Should().Be(1);
        resultList[1].Id.Should().Be(2);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnEmptyList_WhenApiReturnsEmptyResponse()
    {
        // Arrange
        const string jsonResponse = "[]"; // Empty response
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _sut.GetProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnEmptyList_WhenApiRequestFails()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _sut.GetProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProductsAsync_ShouldCallCorrectApiUrl()
    {
        // Arrange
        var jsonResponse = JsonSerializer.Serialize(_testProducts);
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri != null &&
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString() == _apiSettingsMock.Value.ProductApiUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse)
            .Verifiable();

        // Act
        await _sut.GetProductsAsync();

        // Assert
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null &&
                req.Method == HttpMethod.Get &&
                req.RequestUri.ToString() == _apiSettingsMock.Value.ProductApiUrl),
            ItExpr.IsAny<CancellationToken>());
    }
}
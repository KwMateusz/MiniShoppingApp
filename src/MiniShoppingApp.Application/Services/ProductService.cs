using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ICollection<Product>> GetProductsAsync()
    {
        return await _productRepository.GetProductsAsync();
    }
}
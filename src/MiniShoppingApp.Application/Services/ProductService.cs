using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.Application.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<(ICollection<Product> Products, int TotalPages)> GetProductsAsync(int page = 1, int pageSize = 5)
    {
        var products = await productRepository.GetProductsAsync();

        var totalPages = (int)Math.Ceiling(products.Count / (double)pageSize);
        var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return (pagedProducts, totalPages);
    }
}
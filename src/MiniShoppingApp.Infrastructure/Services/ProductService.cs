using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.Infrastructure.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private const int DefaultPageSize = 5; // Default when invalid size is provided

    public async Task<(ICollection<Product> Products, int TotalPages)> GetProductsAsync(int page = 1, int pageSize = 5)
    {
        // Ensure valid page and pageSize values
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1)
        {
            pageSize = DefaultPageSize;
        }

        var products = await productRepository.GetProductsAsync();

        var totalPages = (int)Math.Ceiling(products.Count / (double)pageSize);
        var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return (pagedProducts, totalPages);
    }
}
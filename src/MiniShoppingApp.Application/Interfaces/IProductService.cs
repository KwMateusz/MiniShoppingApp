using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.Application.Interfaces;

public interface IProductService
{
    Task<(ICollection<Product> Products, int TotalPages)> GetProductsAsync(int page = 1, int pageSize = 5);
}
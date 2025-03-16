using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.Application.Interfaces;

public interface IProductService
{
    Task<ICollection<Product>> GetProductsAsync();
}
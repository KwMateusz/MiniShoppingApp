using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.Application.Interfaces;

public interface IProductRepository
{
    Task<ICollection<Product>> GetProductsAsync();
}
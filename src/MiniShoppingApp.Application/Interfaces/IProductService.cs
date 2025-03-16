using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.Application.Interfaces;

public interface IProductService
{
    Task<(ICollection<Product> Products, int TotalPages)> GetProductsAsync(int page = 1, int pageSize = 5);

    ICollection<CartItem> GetCartItems();

    int GetCartItemCount();

    int GetProductQuantity(int productId);

    Task<bool> AddToCart(int productId);

    bool RemoveFromCart(int productId);
}
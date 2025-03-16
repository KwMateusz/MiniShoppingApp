using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.Application.Interfaces;

public interface ICartService
{
    ICollection<CartItem> GetCartItems();

    int GetCartItemCount();

    int GetProductQuantity(int productId);

    Task<bool> AddToCart(int productId);

    bool RemoveFromCart(int productId);
}
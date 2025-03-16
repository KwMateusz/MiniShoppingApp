using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.Application.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private static readonly List<CartItem> Cart = new();

    public async Task<(ICollection<Product> Products, int TotalPages)> GetProductsAsync(int page = 1, int pageSize = 5)
    {
        var products = await productRepository.GetProductsAsync();

        int totalPages = (int)Math.Ceiling(products.Count / (double)pageSize);
        var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return (pagedProducts, totalPages);
    }

    public ICollection<CartItem> GetCartItems()
    {
        return Cart;
    }

    public int GetCartItemCount()
    {
        return Cart.Sum(item => item.Quantity);
    }

    public int GetProductQuantity(int productId)
    {
        return Cart.FirstOrDefault(item => item.Product.Id == productId)?.Quantity ?? 0;
    }

    public async Task<bool> AddToCart(int productId)
    {
        var products = await productRepository.GetProductsAsync();
        var product = products.FirstOrDefault(p => p.Id == productId);

        if (product == null)
        {
            return false;
        }

        var existingItem = Cart.FirstOrDefault(item => item.Product.Id == productId);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            Cart.Add(new CartItem { Product = product, Quantity = 1 });
        }

        return true;
    }

    public bool RemoveFromCart(int productId)
    {
        var existingItem = Cart.FirstOrDefault(item => item.Product.Id == productId);
        if (existingItem == null)
        {
            return false;
        }

        if (existingItem.Quantity > 1)
        {
            existingItem.Quantity--;
        }
        else
        {
            Cart.Remove(existingItem);
        }

        return true;
    }
}
using Microsoft.AspNetCore.Http; // Required for ISession
using System.Text.Json; // Required for JSON serialization
using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly IProductRepository _productRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISession _session;

    private const string CartSessionKey = "ShoppingCart";

    public CartService(IProductRepository productRepository, IHttpContextAccessor httpContextAccessor)
    {
        _productRepository = productRepository;
        _httpContextAccessor = httpContextAccessor;
        _session = _httpContextAccessor.HttpContext.Session;
    }

    private ISession GetSession()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            throw new Exception("HTTP Context is not available. Ensure session middleware is configured.");
        }

        return httpContext.Session;
    }

    private List<CartItem> GetCartFromSession()
    {
        var session = GetSession();
        var cartJson = session.GetString(CartSessionKey);

        if (string.IsNullOrEmpty(cartJson)) // Fix for empty session value
        {
            return new List<CartItem>(); // Return empty cart instead of attempting to deserialize `null`
        }

        return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
    }


    private void SaveCartToSession(List<CartItem> cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        _session.SetString(CartSessionKey, cartJson);
    }

    public ICollection<CartItem> GetCartItems() => GetCartFromSession();

    public int GetCartItemCount() => GetCartFromSession().Sum(item => item.Quantity);

    public int GetProductQuantity(int productId) =>
        GetCartFromSession().FirstOrDefault(item => item.Product.Id == productId)?.Quantity ?? 0;

    public async Task<bool> AddToCart(int productId)
    {
        var products = await _productRepository.GetProductsAsync();
        var product = products?.FirstOrDefault(p => p.Id == productId);

        if (product == null)
        {
            return false;
        }

        var cart = GetCartFromSession();
        var existingItem = cart.FirstOrDefault(item => item.Product.Id == productId);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Add(new CartItem { Product = product, Quantity = 1 });
        }

        SaveCartToSession(cart);
        return true;
    }

    public bool RemoveFromCart(int productId)
    {
        var cart = GetCartFromSession();
        var existingItem = cart.FirstOrDefault(item => item.Product.Id == productId);

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
            cart.Remove(existingItem);
        }

        SaveCartToSession(cart);
        return true;
    }
}
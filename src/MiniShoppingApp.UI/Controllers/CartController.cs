using Microsoft.AspNetCore.Mvc;
using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Domain.Models;

namespace MiniShoppingApp.UI.Controllers;

public class CartController : Controller
{
    private static List<CartItem> Cart = new List<CartItem>();
    private readonly IProductService _productService;

    public CartController(IProductService productService)
    {
        _productService = productService;
    }

    public IActionResult Index()
    {
        ViewBag.TotalPrice = Cart.Sum(item => item.Product.Price * item.Quantity);
        return View(Cart);
    }

    [HttpGet]
    public IActionResult GetCartCount()
    {
        int totalItems = Cart.Sum(item => item.Quantity);
        return Json(new { cartCount = totalItems });
    }

    [HttpGet]
    public IActionResult GetProductQuantity(int productId)
    {
        int quantity = Cart.FirstOrDefault(item => item.Product.Id == productId)?.Quantity ?? 0;
        return Json(new { productQuantity = quantity });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var products = await _productService.GetProductsAsync();
        var product = products.FirstOrDefault(p => p.Id == productId);

        if (product != null)
        {
            var existingItem = Cart.FirstOrDefault(item => item.Product.Id == productId);
            if (existingItem != null)
            {
                existingItem.Quantity++; // Increase quantity if the product is already in the cart
            }
            else
            {
                Cart.Add(new CartItem { Product = product, Quantity = 1 }); // Add new product if not in cart
            }
        }

        return Json(new { success = true, cartCount = Cart.Sum(item => item.Quantity), productQuantity = Cart.FirstOrDefault(item => item.Product.Id == productId)?.Quantity ?? 0 });
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var existingItem = Cart.FirstOrDefault(item => item.Product.Id == productId);
        if (existingItem != null)
        {
            if (existingItem.Quantity > 1)
            {
                existingItem.Quantity--; // Reduce quantity
            }
            else
            {
                Cart.Remove(existingItem); // Remove item if quantity is 1
            }
        }

        return Json(new { success = true, cartCount = Cart.Sum(item => item.Quantity), productQuantity = Cart.FirstOrDefault(item => item.Product.Id == productId)?.Quantity ?? 0 });
    }
}
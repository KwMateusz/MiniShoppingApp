using Microsoft.AspNetCore.Mvc;
using MiniShoppingApp.Models;
using MiniShoppingApp.Services;

namespace MiniShoppingApp.Controllers;

public class CartController : Controller
{
    private static List<Product> Cart = new List<Product>();
    private readonly ProductService _productService;

    public CartController(ProductService productService)
    {
        _productService = productService;
    }

    public IActionResult Index()
    {
        ViewBag.TotalPrice = Cart.Sum(p => p.Price);
        return View(Cart);
    }

    [HttpGet]
    public IActionResult GetCartCount()
    {
        return Json(new { cartCount = Cart.Count });
    }


    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var products = await _productService.GetProductsAsync();
        var product = products.FirstOrDefault(p => p.Id == productId);

        if (product != null)
        {
            Cart.Add(product);
        }

        // Return JSON response with updated cart count
        return Json(new { success = true, cartCount = Cart.Count });
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var product = Cart.FirstOrDefault(p => p.Id == productId);
        if (product != null)
        {
            Cart.Remove(product);
        }
        return RedirectToAction("Index");
    }
}
using Microsoft.AspNetCore.Mvc;
using MiniShoppingApp.Application.Interfaces;

namespace MiniShoppingApp.UI.Controllers;

public class CartController(IProductService productService) : Controller
{
    public IActionResult Index()
    {
        ViewBag.TotalPrice = productService.GetCartItems().Sum(item => item.Product.Price * item.Quantity);

        return View(productService.GetCartItems().ToList());
    }

    [HttpGet]
    public IActionResult GetCartCount()
    {
        return Json(new { cartCount = productService.GetCartItemCount() });
    }

    [HttpGet]
    public IActionResult GetProductQuantity(int productId)
    {
        return Json(new { productQuantity = productService.GetProductQuantity(productId) });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var success = await productService.AddToCart(productId);

        return Json(new { success, cartCount = productService.GetCartItemCount(), productQuantity = productService.GetProductQuantity(productId) });
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var success = productService.RemoveFromCart(productId);

        return Json(new { success, cartCount = productService.GetCartItemCount(), productQuantity = productService.GetProductQuantity(productId) });
    }
}
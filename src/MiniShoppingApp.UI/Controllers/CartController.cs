using Microsoft.AspNetCore.Mvc;
using MiniShoppingApp.Application.Interfaces;

namespace MiniShoppingApp.UI.Controllers;

public class CartController(ICartService cartService) : Controller
{
    public IActionResult Index()
    {
        ViewBag.TotalPrice = cartService.GetCartItems().Sum(item => item.Product.Price * item.Quantity);

        return View(cartService.GetCartItems().ToList());
    }

    [HttpGet]
    public IActionResult GetCartCount()
    {
        return Json(new { cartCount = cartService.GetCartItemCount() });
    }

    [HttpGet]
    public IActionResult GetProductQuantity(int productId)
    {
        return Json(new { productQuantity = cartService.GetProductQuantity(productId) });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var success = await cartService.AddToCart(productId);

        return Json(new { success, cartCount = cartService.GetCartItemCount(), productQuantity = cartService.GetProductQuantity(productId) });
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var success = cartService.RemoveFromCart(productId);

        return Json(new { success, cartCount = cartService.GetCartItemCount(), productQuantity = cartService.GetProductQuantity(productId) });
    }
}
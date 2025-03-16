using Microsoft.AspNetCore.Mvc;
using MiniShoppingApp.Application.Interfaces;

namespace MiniShoppingApp.UI.Controllers;

public class ProductController(IProductService productService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
    {
        var (products, totalPages) = await productService.GetProductsAsync(page, pageSize);

        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentPage = page;

        return View(products.ToList());
    }
}
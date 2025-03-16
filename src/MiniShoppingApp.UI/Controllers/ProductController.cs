﻿using Microsoft.AspNetCore.Mvc;
using MiniShoppingApp.Application.Interfaces;

namespace MiniShoppingApp.UI.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
    {
        var products = await _productService.GetProductsAsync();
        var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.TotalPages = (int)Math.Ceiling(products.Count / (double)pageSize);
        ViewBag.CurrentPage = page;

        return View(pagedProducts);
    }
}
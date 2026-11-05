using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Infrastructure;

namespace OnlineShop.MVC.Controllers;

public sealed class HomeController : Controller
{
    private readonly OnlineShopContext _context;

    public HomeController(OnlineShopContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        ViewBag.ProductCount = await _context.Products.CountAsync();
        ViewBag.CustomerCount = await _context.Customers.CountAsync();
        return View();
    }

    public IActionResult Error() => View();
}

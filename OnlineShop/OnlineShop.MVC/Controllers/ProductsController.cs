using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Infrastructure;
using OnlineShop.Infrastructure.Models;
using OnlineShop.MVC.Models;

namespace OnlineShop.MVC.Controllers;

public sealed class ProductsController : Controller
{
    private readonly OnlineShopContext _context;

    public ProductsController(OnlineShopContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        if (id is null) return NotFound();
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound() : View(product);
    }

    public IActionResult Create() => View(new ProductFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormModel form)
    {
        if (!ModelState.IsValid) return View(form);

        _context.Products.Add(new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = form.Name.Trim(),
            Price = form.Price
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null) return NotFound();
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();
        return View(new ProductFormModel { Name = product.Name, Price = product.Price });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProductFormModel form)
    {
        if (!ModelState.IsValid) return View(form);
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) return NotFound();

        // Update only editable fields and preserve PhoneModel/LaptopModel-specific data.
        product.Name = form.Name.Trim();
        product.Price = form.Price;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null) return NotFound();
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound() : View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) return NotFound();
        _context.Products.Remove(product);
        try
        {
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            return Conflict("Не можна видалити товар, який використовується в замовленні.");
        }
    }
}

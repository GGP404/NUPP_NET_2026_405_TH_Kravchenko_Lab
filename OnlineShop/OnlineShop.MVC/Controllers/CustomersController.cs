using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Infrastructure;
using OnlineShop.Infrastructure.Models;
using OnlineShop.MVC.Models;

namespace OnlineShop.MVC.Controllers;

public sealed class CustomersController : Controller
{
    private readonly OnlineShopContext _context;

    public CustomersController(OnlineShopContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var customers = await _context.Customers.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        return View(customers);
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        if (id is null) return NotFound();
        var customer = await _context.Customers.AsNoTracking().Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id);
        return customer is null ? NotFound() : View(customer);
    }

    public IActionResult Create() => View(new CustomerFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerFormModel form)
    {
        if (!ModelState.IsValid) return View(form);
        _context.Customers.Add(new CustomerModel
        {
            Id = Guid.NewGuid(),
            Name = form.Name.Trim(),
            Email = form.Email.Trim(),
            Age = form.Age
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null) return NotFound();
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null) return NotFound();
        return View(new CustomerFormModel { Name = customer.Name, Email = customer.Email, Age = customer.Age });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CustomerFormModel form)
    {
        if (!ModelState.IsValid) return View(form);
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null) return NotFound();
        customer.Name = form.Name.Trim();
        customer.Email = form.Email.Trim();
        customer.Age = form.Age;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null) return NotFound();
        var customer = await _context.Customers.AsNoTracking().Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id);
        return customer is null ? NotFound() : View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null) return NotFound();
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

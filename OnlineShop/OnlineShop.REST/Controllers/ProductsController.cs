using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Common.Services;
using OnlineShop.Infrastructure.Models;
using OnlineShop.REST.Models;

namespace OnlineShop.REST.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public sealed class ProductsController : ControllerBase
{
    private readonly ICrudServiceAsync<ProductModel> _products;

    public ProductsController(ICrudServiceAsync<ProductModel> products)
    {
        _products = products;
    }

    /// <summary>Returns one page of products.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProductResponseModel>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest(new { message = "page must be >= 1 and pageSize must be between 1 and 100." });

        var products = await _products.ReadAllAsync(page, pageSize);
        return Ok(products.Select(ToResponse));
    }

    /// <summary>Returns a product by its GUID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponseModel>> GetById(Guid id)
    {
        var product = await _products.ReadAsync(id);
        return product is null ? NotFound() : Ok(ToResponse(product));
    }

    /// <summary>Creates a new product.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponseModel>> Create(ProductCreateModel model)
    {
        if ((model.Type == "Phone" && (string.IsNullOrWhiteSpace(model.Brand) || model.Memory is null || string.IsNullOrWhiteSpace(model.OperatingSystem))) ||
            (model.Type == "Laptop" && (string.IsNullOrWhiteSpace(model.Brand) || model.Ram is null || string.IsNullOrWhiteSpace(model.Processor))))
        {
            return BadRequest(new { message = "Phone requires Brand, Memory, and OperatingSystem; Laptop requires Brand, Ram, and Processor." });
        }

        ProductModel product = model.Type switch
        {
            "Phone" => new PhoneModel
            {
                Id = Guid.NewGuid(), Name = model.Name.Trim(), Price = model.Price,
                Brand = model.Brand!.Trim(), Memory = model.Memory!.Value,
                OperatingSystem = model.OperatingSystem!.Trim()
            },
            "Laptop" => new LaptopModel
            {
                Id = Guid.NewGuid(), Name = model.Name.Trim(), Price = model.Price,
                Brand = model.Brand!.Trim(), Ram = model.Ram!.Value,
                Processor = model.Processor!.Trim()
            },
            _ => new ProductModel { Id = Guid.NewGuid(), Name = model.Name.Trim(), Price = model.Price }
        };

        if (!await _products.CreateAsync(product))
            return Conflict(new { message = "A product with this ID already exists." });

        var response = ToResponse(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }

    /// <summary>Updates an existing product. Its inheritance type cannot be changed.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, ProductUpdateModel model)
    {
        var product = await _products.ReadAsync(id);
        if (product is null)
            return NotFound();

        string existingType = GetTypeName(product);
        if (!string.Equals(existingType, model.Type, StringComparison.Ordinal))
            return BadRequest(new { message = "The product type cannot be changed. Send the existing Type value." });

        if ((product is PhoneModel && (string.IsNullOrWhiteSpace(model.Brand) || model.Memory is null || string.IsNullOrWhiteSpace(model.OperatingSystem))) ||
            (product is LaptopModel && (string.IsNullOrWhiteSpace(model.Brand) || model.Ram is null || string.IsNullOrWhiteSpace(model.Processor))))
        {
            return BadRequest(new { message = "Phone requires Brand, Memory, and OperatingSystem; Laptop requires Brand, Ram, and Processor." });
        }

        product.Name = model.Name.Trim();
        product.Price = model.Price;
        if (product is PhoneModel phone)
        {
            phone.Brand = model.Brand!.Trim();
            phone.Memory = model.Memory!.Value;
            phone.OperatingSystem = model.OperatingSystem!.Trim();
        }
        else if (product is LaptopModel laptop)
        {
            laptop.Brand = model.Brand!.Trim();
            laptop.Ram = model.Ram!.Value;
            laptop.Processor = model.Processor!.Trim();
        }

        return await _products.UpdateAsync(product) ? NoContent() : NotFound();
    }

    /// <summary>Deletes an existing product.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _products.ReadAsync(id);
        if (product is null)
            return NotFound();

        try
        {
            return await _products.RemoveAsync(product) ? NoContent() : NotFound();
        }
        catch (DbUpdateException)
        {
            // The Lab 3 schema restricts deleting products referenced by order items.
            return Conflict(new { message = "The product is referenced by an order item and cannot be deleted." });
        }
    }

    private static ProductResponseModel ToResponse(ProductModel product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Type = GetTypeName(product),
        Brand = product switch { PhoneModel phone => phone.Brand, LaptopModel laptop => laptop.Brand, _ => null },
        Memory = product is PhoneModel phoneModel ? phoneModel.Memory : null,
        OperatingSystem = product is PhoneModel phoneOs ? phoneOs.OperatingSystem : null,
        Ram = product is LaptopModel laptopModel ? laptopModel.Ram : null,
        Processor = product is LaptopModel laptopCpu ? laptopCpu.Processor : null
    };

    private static string GetTypeName(ProductModel product) => product switch
    {
        PhoneModel => "Phone",
        LaptopModel => "Laptop",
        _ => "Product"
    };
}

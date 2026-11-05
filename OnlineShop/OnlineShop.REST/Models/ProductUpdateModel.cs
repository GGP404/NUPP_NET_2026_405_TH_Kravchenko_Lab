using System.ComponentModel.DataAnnotations;

namespace OnlineShop.REST.Models;

public sealed class ProductUpdateModel
{
    [Required, StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1_000_000_000)]
    public double Price { get; set; }

    [Required, RegularExpression("^(Product|Phone|Laptop)$", ErrorMessage = "Type must be Product, Phone, or Laptop.")]
    public string Type { get; set; } = "Product";

    [StringLength(100)]
    public string? Brand { get; set; }

    [Range(1, 4096)]
    public int? Memory { get; set; }

    [StringLength(100)]
    public string? OperatingSystem { get; set; }

    [Range(1, 1024)]
    public int? Ram { get; set; }

    [StringLength(100)]
    public string? Processor { get; set; }
}

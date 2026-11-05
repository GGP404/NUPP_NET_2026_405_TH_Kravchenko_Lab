namespace OnlineShop.REST.Models;

/// <summary>Only the product fields exposed by the REST API.</summary>
public sealed class ProductResponseModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public double Price { get; init; }
    public string Type { get; init; } = "Product";
    public string? Brand { get; init; }
    public int? Memory { get; init; }
    public string? OperatingSystem { get; init; }
    public int? Ram { get; init; }
    public string? Processor { get; init; }
}

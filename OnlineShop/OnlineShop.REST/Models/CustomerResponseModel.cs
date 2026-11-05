namespace OnlineShop.REST.Models;

/// <summary>Customer data returned by the REST API, without order navigation properties.</summary>
public sealed class CustomerResponseModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int Age { get; init; }
}

using System.ComponentModel.DataAnnotations;

namespace OnlineShop.REST.Models;

public sealed class CustomerCreateModel
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Range(1, 120)]
    public int Age { get; set; }
}

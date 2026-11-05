using System.ComponentModel.DataAnnotations;

namespace OnlineShop.MVC.Models;

public sealed class ProductFormModel
{
    [Required, StringLength(200, MinimumLength = 2, ErrorMessage = "Назва має містити від 2 до 200 символів.")]
    [Display(Name = "Назва")]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1_000_000_000, ErrorMessage = "Ціна має бути невід’ємною.")]
    [Display(Name = "Ціна, грн")]
    public double Price { get; set; }
}

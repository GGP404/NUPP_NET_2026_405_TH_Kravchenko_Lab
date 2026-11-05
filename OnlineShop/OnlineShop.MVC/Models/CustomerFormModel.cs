using System.ComponentModel.DataAnnotations;

namespace OnlineShop.MVC.Models;

public sealed class CustomerFormModel
{
    [Required, StringLength(150, MinimumLength = 2, ErrorMessage = "Ім’я має містити від 2 до 150 символів.")]
    [Display(Name = "Ім’я")]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    [Display(Name = "Електронна пошта")]
    public string Email { get; set; } = string.Empty;

    [Range(1, 120, ErrorMessage = "Вік має бути від 1 до 120 років.")]
    [Display(Name = "Вік")]
    public int Age { get; set; }
}

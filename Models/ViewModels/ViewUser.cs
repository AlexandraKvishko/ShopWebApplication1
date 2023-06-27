using System.ComponentModel.DataAnnotations;

namespace ShopWebApplication1.Models.ViewModels;

public partial class ViewProfile
{ 
    [Required(AllowEmptyStrings = false, ErrorMessage = "Не вказано ім'я")]
    public string? Name { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Не вказано прізвище")]
    public string? Surname { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Не вказано по батькові")]
    public string? Patronymic { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Не вказано адресу")]
    public string? Address { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Не вказано телефон")]
    public string? PhoneNumber { get; set; }
}

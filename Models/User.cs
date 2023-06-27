using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopWebApplication1.Models;

public partial class User
{ 
    public int Id { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Не вказано e-mail")]
    [EmailAddress(ErrorMessage = "Не вірний e-mail")]
    public string Email { get; set; } = null!;

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Patronymic { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Не вказано пароль")]
    [StringLength(maximumLength: 15, MinimumLength = 5, ErrorMessage = "Пароль повинен бути від от 5 до 15 символів")]
    public string? Password { get; set; }
    
    public int Role { get; set; }

    public string? Token { get; set; }

    public virtual ICollection<Baskett> Basketts { get; set; } = new List<Baskett>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

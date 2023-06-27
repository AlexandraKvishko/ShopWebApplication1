using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopWebApplication1.Models;

public partial class Category
{
    public int Id { get; set; }
    [Display(Name = "Категорія")]

    public string Name { get; set; }
    [Display(Name = "Інформація про категорію")]

    public virtual ICollection<Good> Goods { get; set; } = new List<Good>();
}

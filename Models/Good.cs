using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopWebApplication1.Models;

public partial class Good
{
    public int Id { get; set; }

    public int CatId { get; set; }

    public string? Barcode { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string Descrip { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Baskett> Basketts { get; set; } = new List<Baskett>();

    public virtual Category Cat { get; } = null!;

    public virtual ICollection<OrderGood> OrderGoods { get; set; } = new List<OrderGood>();
}


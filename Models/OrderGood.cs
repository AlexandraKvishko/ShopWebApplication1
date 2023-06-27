using System;
using System.Collections.Generic;

namespace ShopWebApplication1.Models;

public partial class OrderGood
{
    public int Id { get; set; }

    public int GoodId { get; set; }

    public int Quatity { get; set; }

    public int OrderId { get; set; }

    public virtual Good Good { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace ShopWebApplication1.Models;

public partial class Baskett
{
    public int Id { get; set; }

    public int GoodsId { get; set; }

    public int UserId { get; set; }

    public int Quantity { get; set; }

    public virtual Good Goods { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

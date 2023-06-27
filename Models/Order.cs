using System;
using System.Collections.Generic;

namespace ShopWebApplication1.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime Data { get; set; }

    public int StateId { get; set; }

    public virtual ICollection<OrderGood> OrderGoods { get; set; } = new List<OrderGood>();

    public virtual OrderStatus State { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

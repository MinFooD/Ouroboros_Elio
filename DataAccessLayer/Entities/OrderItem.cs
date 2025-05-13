using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class OrderItem
{
    public Guid OrderItemId { get; set; }

    public Guid? OrderId { get; set; }

    public bool ProductType { get; set; }

    public Guid? DesignId { get; set; }

    public Guid? CustomBraceletId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual CustomBracelet? CustomBracelet { get; set; }

    public virtual Design? Design { get; set; }

    public virtual Order? Order { get; set; }
}

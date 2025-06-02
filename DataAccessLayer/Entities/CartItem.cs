using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class CartItem
{
    public Guid CartItemId { get; set; }

    public Guid? CartId { get; set; }

    public bool ProductType { get; set; }

    public Guid DesignId { get; set; }

    public Guid? CustomBraceletId { get; set; }

    public int Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual CustomBracelet? CustomBracelet { get; set; }

    public virtual Design? Design { get; set; }
}

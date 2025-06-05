using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Charm
{
    public Guid CharmId { get; set; }

    public string? Name { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int InternalCode { get; set; }

    public decimal? CapitalExpense { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<CustomBraceletCharm> CustomBraceletCharms { get; set; } = new List<CustomBraceletCharm>();
}

using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Category
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<CustomBracelet> CustomBracelets { get; set; } = new List<CustomBracelet>();

    public virtual ICollection<Design> Designs { get; set; } = new List<Design>();
}

using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class DesignImage
{
    public Guid ImageId { get; set; }

    public Guid DesignId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual Design Design { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class CustomBraceletCharm
{
    public Guid CustomBraceletId { get; set; }

    public Guid CharmId { get; set; }

    public int OrdinalNumber { get; set; }

    public virtual Charm Charm { get; set; } = null!;

    public virtual CustomBracelet CustomBracelet { get; set; } = null!;
}

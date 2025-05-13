using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Collection
{
    public Guid CollectionId { get; set; }

    public string CollectionName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}

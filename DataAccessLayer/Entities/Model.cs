using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Model
{
    public Guid ModelId { get; set; }

    public Guid TopicId { get; set; }

    public string ModelName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Design> Designs { get; set; } = new List<Design>();

    public virtual Topic Topic { get; set; } = null!;
}

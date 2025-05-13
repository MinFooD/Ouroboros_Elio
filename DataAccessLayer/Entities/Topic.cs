using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Topic
{
    public Guid TopicId { get; set; }

    public Guid CollectionId { get; set; }

    public string TopicName { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Collection Collection { get; set; } = null!;

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}

using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid? OrderId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Status { get; set; }

    public virtual Order? Order { get; set; }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities;

public class ContactMessage
{
    [Key]
    public Guid MessageId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [StringLength(200)]
    public string Subject { get; set; }

    [Required]
    [StringLength(2000)]
    public string Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; } = false;
}
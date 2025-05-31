using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.Dtos.ContactDtos;

public class ContactMessageCreateDto
{
    [Required(ErrorMessage = "Vui lòng nhập tên của bạn.")]
    [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập email của bạn.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
    public string Email { get; set; }

    [StringLength(200, ErrorMessage = "Chủ đề không được vượt quá 200 ký tự.")]
    public string Subject { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập nội dung tin nhắn.")]
    [StringLength(2000, ErrorMessage = "Nội dung không được vượt quá 2000 ký tự.")]
    public string Message { get; set; }
}
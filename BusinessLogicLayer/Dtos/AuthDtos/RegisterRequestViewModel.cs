using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.Dtos.AuthDtos;

public class RegisterRequestViewModel
{
    [Required(ErrorMessage = "Username is required.")]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters or digits.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    public string UserName { get; set; }

    [DataType(DataType.EmailAddress)]
    public string Gmail { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}

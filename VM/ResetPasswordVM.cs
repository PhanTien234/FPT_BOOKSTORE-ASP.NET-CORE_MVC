using System.ComponentModel.DataAnnotations;

namespace FPT_BOOKSTORE.VM;

public class ResetPasswordVM
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirm password must match")]
    public string ConfirmPassword { get; set; }

    public string Token { get; set; }
}
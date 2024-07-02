using System.ComponentModel.DataAnnotations;

namespace api;

public class SigninRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}

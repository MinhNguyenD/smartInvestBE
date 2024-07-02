using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace api;

public class RegisterRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]

    public string Username { get; set; }
    [Required]
    public string Password { get; set; }

}

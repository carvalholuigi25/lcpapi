namespace lcpapi.Models.UsersAuth;
using System.ComponentModel.DataAnnotations;

public class AuthenticateRequest
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
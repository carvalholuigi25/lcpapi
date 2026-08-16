namespace lcpapi.Models.UsersAuth;

using System.Text.Json.Serialization;
using lcpapi.Models;

public class AuthenticateResponse
{
    public int? Id { get; set; }
    public string DisplayName { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string JwtToken { get; set; }
    public string? DateBirthday { get; set; } = DateTime.Now.ToString();

    [JsonIgnore] // refresh token is returned in http only cookie
    public string RefreshToken { get; set; }

    public AuthenticateResponse(User user, string jwtToken, string refreshToken)
    {
        Id = user.Id;
        DisplayName = user.DisplayName!;
        Role = user.Role.ToString()!;
        Username = user.Username;
        DateBirthday = user.DateBirthday;
        JwtToken = jwtToken;
        RefreshToken = refreshToken;
    }
}
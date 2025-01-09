namespace lcpapi.Models.UsersAuth;

using System.Text.Json.Serialization;
using lcpapi.Models;

public class AuthenticateResponse
{
    public int? Id { get; set; }
    public string DisplayName { get; set; }
    public string Username { get; set; }
    public string JwtToken { get; set; }

    [JsonIgnore] // refresh token is returned in http only cookie
    public string RefreshToken { get; set; }

    public AuthenticateResponse(User user, string jwtToken, string refreshToken)
    {
        Id = user.Id;
        DisplayName = user.DisplayName!;
        Username = user.Username;
        JwtToken = jwtToken;
        RefreshToken = refreshToken;
    }
}
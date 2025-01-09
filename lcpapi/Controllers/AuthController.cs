using Microsoft.AspNetCore.Mvc;
using lcpapi.Authorization;
using lcpapi.Models.UsersAuth;
using lcpapi.Services;
// using lcpapi.Attributes;

namespace lcpapi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="model"></param>
    /// <returns>User login</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/auth/authenticate
    ///     {
    ///         "username": "string",
    ///         "password": "string"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the user is logged in or not by his authentication credientials</response>
    /// <response code="400">If the user is not logged in by his authentication credientials</response>
    [AllowAnonymous]
    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model, IpAddress());
        SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    /// <summary>
    /// Refresh Token 
    /// </summary>
    /// <returns>Refresh token and updates his user session</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/auth/refresh-token
    ///
    /// </remarks>
    /// <response code="201">Returns the token info and updates his user session</response>
    /// <response code="400">If the user is not updated about his session and token info</response>
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [IgnoreAPI]
    public IActionResult RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = _userService.RefreshToken(refreshToken!, IpAddress());
        SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    /// <summary>
    /// Revoke Token 
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Revokes token and removes his user session</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/auth/revoke-token
    ///     {
    ///         "token": "string"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the token info and removed the current user session</response>
    /// <response code="400">If the user is not removed about his session and token info</response>
    // [Authorize(Roles = "admin,moderator")]
    [Authorize(Policy = "StaffOnly")]
    [HttpPost("revoke-token")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [IgnoreAPI]
    public IActionResult RevokeToken(RevokeTokenRequest model)
    {
        // accept refresh token in request body or cookie
        var token = model.Token ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Token is required" });

        _userService.RevokeToken(token, IpAddress());
        return Ok(new { message = "Token revoked" });
    }

    /// <summary>
    /// Gets all list of users 
    /// </summary>
    /// <returns>Gets all list of users</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/auth
    ///
    /// </remarks>
    /// <response code="201">Returns of all list of users</response>
    /// <response code="400">If the user list is empty</response>
    // [Authorize(Roles = "admin,moderator")]
    [Authorize(Policy = "StaffOnly")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [IgnoreAPI]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    /// <summary>
    /// Gets all list of users by id 
    /// </summary>
    /// <returns>Gets all list of users by id</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/auth/{id}
    ///
    /// </remarks>
    /// <response code="201">Returns of all list of users by id</response>
    /// <response code="400">If the user list by id is empty</response>
    // [Authorize(Roles = "admin,moderator")]
    [Authorize(Policy = "StaffOnly")]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [IgnoreAPI]
    public IActionResult GetById(int id)
    {
        var user = _userService.GetById(id);
        return Ok(user);
    }

    /// <summary>
    /// Refreshes tokens by user id 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Refreshes tokens the current user session by id</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/auth/{id}/refresh-tokens
    ///
    /// </remarks>
    /// <response code="201">Returns and refreshes the current user session by id</response>
    /// <response code="400">If the current user session by id is empty</response>
    // [Authorize(Roles = "admin,moderator")]
    [Authorize(Policy = "StaffOnly")]
    [HttpGet("{id}/refresh-tokens")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [IgnoreAPI]
    public IActionResult GetRefreshTokens(int id)
    {
        var user = _userService.GetById(id);
        return Ok(user.RefreshTokens);
    }

    // helper methods

    private void SetTokenCookie(string token)
    {
        // append cookie with refresh token to the http response
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private string IpAddress()
    {
        // get source ip address for the current request
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"]!;
        else
            return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
    }
}
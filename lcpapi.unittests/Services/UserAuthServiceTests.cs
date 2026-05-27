using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using lcpapi.Authorization;
using lcpapi.Context;
using lcpapi.Helpers;
using lcpapi.Models;
using lcpapi.Models.UsersAuth;
using lcpapi.Services;

namespace lcpapi.unittests.Services;

public class UserAuthServiceTests
{
    private readonly MyDBContext _context;
    private readonly Mock<IJwtUtils> _jwtUtilsMock;
    private readonly Mock<IOtpService> _otpServiceMock;
    private readonly Mock<IOptions<AppSettings>> _appSettingsMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _userService;

    public UserAuthServiceTests()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c.GetSection(It.IsAny<string>()).Value).Returns("MemoryDB");

        var options = new DbContextOptionsBuilder<MyDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MyDBContext(options, configMock.Object);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _jwtUtilsMock = new Mock<IJwtUtils>();
        _otpServiceMock = new Mock<IOtpService>();
        _appSettingsMock = new Mock<IOptions<AppSettings>>();
        _loggerMock = new Mock<ILogger<UserService>>();

        _appSettingsMock.Setup(x => x.Value).Returns(new AppSettings { RefreshTokenTTL = 7 });

        _userService = new UserService(
            _context,
            _jwtUtilsMock.Object,
            _otpServiceMock.Object,
            _appSettingsMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public void Authenticate_WithValidCredentials_ReturnsAuthenticateResponse()
    {
        var password = "password123";
        var user = new User
        {
            Username = "testuser",
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            RefreshTokens = new List<RefreshToken>()
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        _jwtUtilsMock.Setup(x => x.GenerateJwtToken(It.IsAny<User>())).Returns("jwt-token");
        _jwtUtilsMock.Setup(x => x.GenerateRefreshToken(It.IsAny<string>())).Returns(new RefreshToken { Token = "refresh-token", Expires = DateTime.UtcNow.AddDays(7), Created = DateTime.UtcNow });

        var request = new AuthenticateRequest { Username = "testuser", Password = password };

        var response = _userService.Authenticate(request, "127.0.0.1");

        Assert.NotNull(response);
        Assert.Equal("jwt-token", response.JwtToken);
        Assert.Equal("refresh-token", response.RefreshToken);
    }

    [Fact]
    public void Authenticate_WithInvalidUsername_ThrowsAppException()
    {
        var request = new AuthenticateRequest { Username = "missing", Password = "password" };

        Assert.Throws<AppException>(() => _userService.Authenticate(request, "127.0.0.1"));
    }

    [Fact]
    public void Authenticate_WithInvalidPassword_ThrowsAppException()
    {
        var user = new User
        {
            Username = "testuser",
            Password = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            RefreshTokens = new List<RefreshToken>()
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        var request = new AuthenticateRequest { Username = "testuser", Password = "wrongpassword" };

        Assert.Throws<AppException>(() => _userService.Authenticate(request, "127.0.0.1"));
    }

    [Fact]
    public void RefreshToken_WithValidToken_ReturnsNewTokens()
    {
        var refreshToken = new RefreshToken
        {
            Token = "old-token",
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        var user = new User
        {
            Username = "testuser",
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            RefreshTokens = new List<RefreshToken> { refreshToken }
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        _jwtUtilsMock.Setup(x => x.GenerateRefreshToken(It.IsAny<string>())).Returns(new RefreshToken { Token = "new-token", Created = DateTime.UtcNow, Expires = DateTime.UtcNow.AddDays(7) });
        _jwtUtilsMock.Setup(x => x.GenerateJwtToken(It.IsAny<User>())).Returns("new-jwt");

        var response = _userService.RefreshToken("old-token", "127.0.0.1");

        Assert.NotNull(response);
        Assert.Equal("new-jwt", response.JwtToken);
        Assert.Equal("new-token", response.RefreshToken);
    }

    [Fact]
    public void RefreshToken_WithInvalidToken_ThrowsAppException()
    {
        Assert.Throws<AppException>(() => _userService.RefreshToken("invalid-token", "127.0.0.1"));
    }

    [Fact]
    public void RevokeToken_WithValidToken_RevokesToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = "token-to-revoke",
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        var user = new User
        {
            Username = "testuser",
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            RefreshTokens = new List<RefreshToken> { refreshToken }
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        _userService.RevokeToken("token-to-revoke", "127.0.0.1");

        Assert.NotNull(refreshToken.Revoked);
        Assert.Equal("127.0.0.1", refreshToken.RevokedByIp);
    }

    [Fact]
    public void RevokeToken_WithInvalidToken_ThrowsAppException()
    {
        Assert.Throws<AppException>(() => _userService.RevokeToken("invalid-token", "127.0.0.1"));
    }

    [Fact]
    public void GetAll_ReturnsUsers()
    {
        _context.Users.AddRange(
            new User { Username = "a", Password = "x" },
            new User { Username = "b", Password = "y" }
        );
        _context.SaveChanges();

        var users = _userService.GetAll();

        Assert.Equal(2, users.Count());
    }

    [Fact]
    public void GetById_WithExistingId_ReturnsUser()
    {
        var user = new User { Username = "abc", Password = "x" };
        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _userService.GetById(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public void GetById_WithMissingId_ThrowsKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => _userService.GetById(999));
    }
}

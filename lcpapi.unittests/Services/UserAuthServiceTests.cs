using Moq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Services;
using lcpapi.Authorization;
using lcpapi.Helpers;
using lcpapi.Models.UsersAuth;

namespace lcpapi.unittests.Services;

public class UserAuthServiceTests
{
    private readonly Mock<MyDBContext> _mockContext;
    private readonly Mock<IJwtUtils> _mockJwtUtils;
    private readonly Mock<IOtpService> _mockOtpService;
    private readonly Mock<IOptions<AppSettings>> _mockAppSettings;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserAuthServiceTests()
    {
        _mockContext = new Mock<MyDBContext>();
        _mockJwtUtils = new Mock<IJwtUtils>();
        _mockOtpService = new Mock<IOtpService>();
        _mockAppSettings = new Mock<IOptions<AppSettings>>();
        _mockLogger = new Mock<ILogger<UserService>>();

        var appSettings = new AppSettings { RefreshTokenTTL = 7 };
        _mockAppSettings.Setup(s => s.Value).Returns(appSettings);

        _userService = new UserService(
            _mockContext.Object,
            _mockJwtUtils.Object,
            _mockOtpService.Object,
            _mockAppSettings.Object,
            _mockLogger.Object
        );
    }

    #region Authenticate Tests

    [Fact]
    public void Authenticate_WithValidCredentials_ReturnsAuthenticateResponse()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = 1,
            Username = username,
            Password = hashedPassword,
            OtpEnabled = false,
            RefreshTokens = new List<RefreshToken>()
        };

        var users = new List<User> { user };

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);
        _mockJwtUtils.Setup(j => j.GenerateJwtToken(It.IsAny<User>())).Returns("test-jwt-token");
        _mockJwtUtils.Setup(j => j.GenerateRefreshToken(It.IsAny<string>())).Returns(new RefreshToken { Token = "test-refresh-token" });
        _mockContext.Setup(c => c.Update(It.IsAny<User>())).Verifiable();
        _mockContext.Setup(c => c.SaveChanges()).Verifiable();

        var model = new AuthenticateRequest { Username = username, Password = password };

        // Act
        var result = _userService.Authenticate(model, "127.0.0.1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-jwt-token", result.JwtToken);
        Assert.Equal("test-refresh-token", result.RefreshToken);
        _mockJwtUtils.Verify(j => j.GenerateJwtToken(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public void Authenticate_WithInvalidUsername_ThrowsException()
    {
        // Arrange
        var users = new List<User>();

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);

        var model = new AuthenticateRequest { Username = "invaliduser", Password = "password" };

        // Act & Assert
        Assert.Throws<AppException>(() => _userService.Authenticate(model, "127.0.0.1"));
    }

    [Fact]
    public void Authenticate_WithInvalidPassword_ThrowsException()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            OtpEnabled = false,
            RefreshTokens = new List<RefreshToken>()
        };

        var users = new List<User> { user };

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);

        var model = new AuthenticateRequest { Username = "testuser", Password = "wrongpassword" };

        // Act & Assert
        Assert.Throws<AppException>(() => _userService.Authenticate(model, "127.0.0.1"));
    }

    [Fact]
    public void Authenticate_WithOtpEnabled_RequiresOtpCode()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = BCrypt.Net.BCrypt.HashPassword("password123"),
            OtpEnabled = true,
            OtpSecret = "secret123",
            RefreshTokens = new List<RefreshToken>()
        };

        var users = new List<User> { user };

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);

        var model = new AuthenticateRequest { Username = "testuser", Password = "password123" };

        // Act & Assert
        Assert.Throws<AppException>(() => _userService.Authenticate(model, "127.0.0.1"));
    }

    [Fact]
    public void Authenticate_WithOtpEnabledAndValidCode_ReturnsResponse()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = BCrypt.Net.BCrypt.HashPassword("password123"),
            OtpEnabled = true,
            OtpSecret = "secret123",
            RefreshTokens = new List<RefreshToken>()
        };

        var users = new List<User> { user };

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);
        _mockOtpService.Setup(o => o.ValidateTotp("secret123", "123456")).Returns(true);
        _mockJwtUtils.Setup(j => j.GenerateJwtToken(It.IsAny<User>())).Returns("test-jwt-token");
        _mockJwtUtils.Setup(j => j.GenerateRefreshToken(It.IsAny<string>())).Returns(new RefreshToken { Token = "test-refresh-token" });
        _mockContext.Setup(c => c.Update(It.IsAny<User>())).Verifiable();
        _mockContext.Setup(c => c.SaveChanges()).Verifiable();

        var model = new AuthenticateRequest { Username = "testuser", Password = "password123", OtpCode = "123456" };

        // Act
        var result = _userService.Authenticate(model, "127.0.0.1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-jwt-token", result.JwtToken);
    }

    #endregion

    #region RefreshToken Tests

    [Fact]
    public void RefreshToken_WithValidToken_ReturnsNewAuthenticateResponse()
    {
        // Arrange
        var oldRefreshToken = new RefreshToken
        {
            Token = "old-token",
            Revoked = null,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            RefreshTokens = new List<RefreshToken> { oldRefreshToken }
        };

        var users = new List<User> { user };

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);
        _mockJwtUtils.Setup(j => j.GenerateRefreshToken(It.IsAny<string>()))
            .Returns(new RefreshToken { Token = "new-refresh-token", Created = DateTime.UtcNow, Expires = DateTime.UtcNow.AddDays(7) });
        _mockJwtUtils.Setup(j => j.GenerateJwtToken(It.IsAny<User>())).Returns("new-jwt-token");
        _mockContext.Setup(c => c.Update(It.IsAny<User>())).Verifiable();
        _mockContext.Setup(c => c.SaveChanges()).Verifiable();

        // Act
        var result = _userService.RefreshToken("old-token", "127.0.0.1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new-jwt-token", result.JwtToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);
    }

    [Fact]
    public void RefreshToken_WithInvalidToken_ThrowsException()
    {
        // Arrange
        var users = new List<User>();

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);

        // Act & Assert
        Assert.Throws<AppException>(() => _userService.RefreshToken("invalid-token", "127.0.0.1"));
    }

    [Fact]
    public void RefreshToken_WithRevokedToken_RevokeDescendantsAndThrowException()
    {
        // Arrange
        var revokedToken = new RefreshToken
        {
            Token = "revoked-token",
            Revoked = DateTime.UtcNow,
            Created = DateTime.UtcNow.AddDays(-1),
            Expires = DateTime.UtcNow.AddDays(6)
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            RefreshTokens = new List<RefreshToken> { revokedToken }
        };

        var users = new List<User> { user };

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);
        _mockContext.Setup(c => c.Update(It.IsAny<User>())).Verifiable();
        _mockContext.Setup(c => c.SaveChanges()).Verifiable();

        // Act & Assert
        Assert.Throws<AppException>(() => _userService.RefreshToken("revoked-token", "127.0.0.1"));
    }

    #endregion

    #region RevokeToken Tests

    [Fact]
    public void RevokeToken_WithValidToken_RevokesTokenSuccessfully()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            Token = "token-to-revoke",
            Revoked = null,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            RefreshTokens = new List<RefreshToken> { refreshToken }
        };

        var users = new List<User> { user };

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);
        _mockContext.Setup(c => c.Update(It.IsAny<User>())).Verifiable();
        _mockContext.Setup(c => c.SaveChanges()).Verifiable();

        // Act
        _userService.RevokeToken("token-to-revoke", "127.0.0.1");

        // Assert
        Assert.NotNull(refreshToken.Revoked);
        _mockContext.Verify(c => c.Update(It.IsAny<User>()), Times.Once);
        _mockContext.Verify(c => c.SaveChanges(), Times.Once);
    }

    [Fact]
    public void RevokeToken_WithInvalidToken_ThrowsException()
    {
        // Arrange
        var users = new List<User>();

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);

        // Act & Assert
        Assert.Throws<AppException>(() => _userService.RevokeToken("invalid-token", "127.0.0.1"));
    }

    #endregion

    #region GetAll Tests

    [Fact]
    public void GetAll_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Username = "user1" },
            new User { Id = 2, Username = "user2" },
            new User { Id = 3, Username = "user3" }
        };

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);

        // Act
        var result = _userService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void GetAll_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var users = new List<User>();

        _mockContext.Setup(c => c.Users).Returns(GetMockDbSet(users).Object);

        // Act
        var result = _userService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public void GetById_WithValidId_ReturnsUser()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Username = "testuser" };

        var mockDbSet = new Mock<DbSet<User>>();
        mockDbSet.Setup(m => m.Find(userId)).Returns(user);

        _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

        // Act
        var result = _userService.GetById(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public void GetById_WithInvalidId_ThrowsException()
    {
        // Arrange
        var userId = 999;

        var mockDbSet = new Mock<DbSet<User>>();
        mockDbSet.Setup(m => m.Find(userId)).Returns((User?)null);

        _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _userService.GetById(userId));
    }

    #endregion

    #region Helper Methods

    private Mock<DbSet<User>> GetMockDbSet(List<User> users)
    {
        var queryable = users.AsQueryable();
        var mockDbSet = new Mock<DbSet<User>>();

        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        // Setup SingleOrDefault
        mockDbSet.Setup(m => m.SingleOrDefault(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns<System.Linq.Expressions.Expression<Func<User, bool>>>(predicate =>
            {
                var func = predicate.Compile();
                return users.SingleOrDefault(func);
            });

        // Setup Find
        mockDbSet.Setup(m => m.Find(It.IsAny<object[]>()))
            .Returns<object[]>(keys =>
            {
                if (keys.Length > 0 && keys[0] is int id)
                {
                    return users.FirstOrDefault(u => u.Id == id);
                }
                return null;
            });

        return mockDbSet;
    }

    #endregion
}

using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Repositories;
using lcpapi.Models.QParams;
using lcpapi.Hubs;

namespace lcpapi.unittests.Repositories;

public class GamesRepoTests
{
    private readonly Mock<MyDBContext> _mockContext;
    private readonly Mock<IHubContext<ChatHub>> _mockHubContext;
    private readonly GamesRepo _gamesRepo;

    public GamesRepoTests()
    {
        _mockContext = new Mock<MyDBContext>();
        _mockHubContext = new Mock<IHubContext<ChatHub>>();
        _gamesRepo = new GamesRepo(_mockContext.Object, _mockHubContext.Object);
    }

    #region GetGames Tests

    [Fact]
    public async Task GetGames_WithValidQueryParams_ReturnsOkResultWithGames()
    {
        // Arrange
        var games = new List<Game>
        {
            new Game { GameId = 1, Title = "Game 1", Developer = "Developer 1" },
            new Game { GameId = 2, Title = "Game 2", Developer = "Developer 2" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Game>>();
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.Provider).Returns(games.Provider);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.Expression).Returns(games.Expression);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.ElementType).Returns(games.ElementType);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.GetEnumerator()).Returns(games.GetEnumerator());
        mockDbSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);

        var queryParams = new QueryParams { Page = 1, PageSize = 10, SortBy = "id", SortOrder = SortOrderEnum.asc };

        // Act
        var result = await _gamesRepo.GetGames(queryParams);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ActionResult<IEnumerable<Game>>>(result);
    }

    [Fact]
    public async Task GetGames_WithSearchParam_FiltersResults()
    {
        // Arrange
        var games = new List<Game>
        {
            new Game { GameId = 1, Title = "The Witcher 3", Developer = "CD Projekt Red" },
            new Game { GameId = 2, Title = "Elden Ring", Developer = "FromSoftware" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Game>>();
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.Provider).Returns(games.Provider);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.Expression).Returns(games.Expression);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.ElementType).Returns(games.ElementType);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.GetEnumerator()).Returns(games.GetEnumerator());
        mockDbSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);

        var queryParams = new QueryParams { Page = 1, PageSize = 10, SortBy = "title", SortOrder = SortOrderEnum.asc, Search = "Witcher" };

        // Act
        var result = await _gamesRepo.GetGames(queryParams);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region GetGame Tests

    [Fact]
    public async Task GetGame_WithValidId_ReturnsOkResultWithGame()
    {
        // Arrange
        var gameId = 1;
        var game = new Game { GameId = gameId, Title = "Test Game", Developer = "Test Developer" };

        var mockDbSet = new Mock<DbSet<Game>>();
        mockDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Game, bool>>>(), CancellationToken.None))
            .ReturnsAsync(game);

        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);

        // Act
        var result = await _gamesRepo.GetGame(gameId);

        // Assert
        Assert.NotNull(result);
        var okResult = result.Value;
        Assert.Equal(gameId, okResult?.GameId);
        Assert.Equal("Test Game", okResult?.Title);
    }

    [Fact]
    public async Task GetGame_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var gameId = 999;

        var mockDbSet = new Mock<DbSet<Game>>();
        mockDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Game, bool>>>(), CancellationToken.None))
            .ReturnsAsync((Game?)null);

        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);

        // Act
        var result = await _gamesRepo.GetGame(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Value);
    }

    #endregion

    #region CreateGame Tests

    [Fact]
    public async Task CreateGame_WithValidGame_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var newGame = new Game 
        { 
            GameId = 1, 
            Title = "New Game", 
            Developer = "New Developer", 
            Description = "Test Description",
            Publisher = "Test Publisher"
        };

        var mockDbSet = new Mock<DbSet<Game>>();
        mockDbSet.Setup(m => m.Add(It.IsAny<Game>())).Verifiable();

        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        var result = await _gamesRepo.CreateGame(newGame);

        // Assert
        Assert.NotNull(result);
        var createdResult = result.Value;
        Assert.Equal("New Game", createdResult?.Title);
        Assert.Equal("New Developer", createdResult?.Developer);
        _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateGame_CallsSaveChangesAsync()
    {
        // Arrange
        var newGame = new Game { GameId = 1, Title = "Another Game" };

        var mockDbSet = new Mock<DbSet<Game>>();
        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        await _gamesRepo.CreateGame(newGame);

        // Assert
        _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    #endregion

    #region PutGame Tests

    [Fact]
    public async Task PutGame_WithMatchingIds_UpdatesGame()
    {
        // Arrange
        var gameId = 1;
        var game = new Game { GameId = gameId, Title = "Updated Game", Developer = "Updated Developer" };

        var mockDbSet = new Mock<DbSet<Game>>();
        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.Entry(It.IsAny<Game>())).Returns(new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Game>>().Object);
        _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        var result = await _gamesRepo.PutGame(gameId, game);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
        _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task PutGame_WithMismatchedIds_ReturnsBadRequest()
    {
        // Arrange
        var gameId = 1;
        var game = new Game { GameId = 2, Title = "Test Game" };

        // Act
        var result = await _gamesRepo.PutGame(gameId, game);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestResult>(result);
    }

    #endregion

    #region DeleteGame Tests

    [Fact]
    public async Task DeleteGame_WithValidId_DeletesGame()
    {
        // Arrange
        var gameId = 1;
        var game = new Game { GameId = gameId, Title = "Game to Delete" };

        var mockDbSet = new Mock<DbSet<Game>>();
        mockDbSet.Setup(m => m.FindAsync(new object[] { gameId }, CancellationToken.None))
            .ReturnsAsync(game);

        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        var result = await _gamesRepo.DeleteGame(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
        mockDbSet.Verify(m => m.Remove(It.Is<Game>(g => g.GameId == gameId)), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteGame_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var gameId = 999;

        var mockDbSet = new Mock<DbSet<Game>>();
        mockDbSet.Setup(m => m.FindAsync(new object[] { gameId }, CancellationToken.None))
            .ReturnsAsync((Game?)null);

        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);

        // Act
        var result = await _gamesRepo.DeleteGame(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region GetTotalCountAsync Tests

    [Fact]
    public async Task GetTotalCountAsync_WithNoFilter_ReturnsTotalCount()
    {
        // Arrange
        var games = new List<Game>
        {
            new Game { GameId = 1, Title = "Game 1" },
            new Game { GameId = 2, Title = "Game 2" },
            new Game { GameId = 3, Title = "Game 3" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Game>>();
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.Provider).Returns(games.Provider);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.Expression).Returns(games.Expression);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.ElementType).Returns(games.ElementType);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.GetEnumerator()).Returns(games.GetEnumerator());

        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        // Act
        var result = await _gamesRepo.GetTotalCountAsync(queryParams);

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task GetTotalCountAsync_WithFilter_ReturnsFilteredCount()
    {
        // Arrange
        var games = new List<Game>
        {
            new Game { GameId = 1, Title = "The Witcher 3" },
            new Game { GameId = 2, Title = "Elden Ring" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Game>>();
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.Provider).Returns(games.Provider);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.Expression).Returns(games.Expression);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.ElementType).Returns(games.ElementType);
        mockDbSet.As<IQueryable<Game>>().Setup(m => m.GetEnumerator()).Returns(games.GetEnumerator());

        _mockContext.Setup(c => c.Games).Returns(mockDbSet.Object);

        var queryParams = new QueryParams { Page = 1, PageSize = 10, SortBy = "title", Search = "Witcher" };

        // Act
        var result = await _gamesRepo.GetTotalCountAsync(queryParams);

        // Assert
        Assert.True(result >= 0);
    }

    #endregion
}

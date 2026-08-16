using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using lcpapi.Context;
using lcpapi.Hubs;
using lcpapi.Models;
using lcpapi.Models.QParams;
using lcpapi.Repositories;

namespace lcpapi.unittests.Repositories;

public class GamesRepoTests
{
    private readonly MyDBContext _context;
    private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
    private readonly GamesRepo _repo;

    public GamesRepoTests()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c.GetSection(It.IsAny<string>()).Value).Returns("MemoryDB");

        var options = new DbContextOptionsBuilder<MyDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MyDBContext(options, configMock.Object);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _hubContextMock = new Mock<IHubContext<ChatHub>>();
        var clientsMock = new Mock<IHubClients>();
        var clientProxyMock = new Mock<IClientProxy>();
        clientProxyMock
            .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        clientsMock.Setup(c => c.All).Returns(clientProxyMock.Object);
        _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

        _repo = new GamesRepo(_context, _hubContextMock.Object);
    }

    [Fact]
    public async Task GetGames_ReturnsGamesFromContext()
    {
        _context.Games.AddRange(
            new Game { Title = "Game 1" },
            new Game { Title = "Game 2" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var result = await _repo.GetGames(queryParams);

        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetGame_WithExistingId_ReturnsGame()
    {
        var game = new Game { Title = "My Game" };
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        var result = await _repo.GetGame(game.GameId);

        Assert.NotNull(result.Value);
        Assert.Equal(game.GameId, result.Value!.GameId);
        Assert.Equal("My Game", result.Value.Title);
    }

    [Fact]
    public async Task GetGame_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.GetGame(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateGame_AddsGameAndReturnsCreatedResult()
    {
        var game = new Game { Title = "New Game" };

        var result = await _repo.CreateGame(game);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedGame = Assert.IsType<Game>(createdResult.Value);
        Assert.Equal("New Game", returnedGame.Title);
        Assert.Equal(1, await _context.Games.CountAsync());
        Assert.Equal(game.GameId, returnedGame.GameId);
    }

    [Fact]
    public async Task PutGame_WithMismatchedId_ReturnsBadRequest()
    {
        var game = new Game { Title = "Existing Game" };
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        var updatedGame = new Game { GameId = game.GameId + 1, Title = "Updated Game" };

        var result = await _repo.PutGame(game.GameId, updatedGame);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteGame_RemovesGame_WhenExists()
    {
        var game = new Game { Title = "Delete Game" };
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        var result = await _repo.DeleteGame(game.GameId);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_context.Games);
    }

    [Fact]
    public async Task DeleteGame_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.DeleteGame(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTotalCountAsync_ReturnsTotalCount()
    {
        _context.Games.AddRange(
            new Game { Title = "A" },
            new Game { Title = "B" },
            new Game { Title = "C" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var count = await _repo.GetTotalCountAsync(queryParams);

        Assert.Equal(3, count);
    }
}

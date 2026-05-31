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

public class MoviesRepoTests
{
    private readonly MyDBContext _context;
    private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
    private readonly MoviesRepo _repo;

    public MoviesRepoTests()
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

        _repo = new MoviesRepo(_context, _hubContextMock.Object);
    }

    [Fact]
    public async Task GetMovies_ReturnsItems()
    {
        _context.Movies.AddRange(
            new Movies { Title = "M1" },
            new Movies { Title = "M2" }
        );
        await _context.SaveChangesAsync();

        var qp = new QueryParams { Page = 1, PageSize = 10 };

        var result = await _repo.GetMovies(qp);

        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task CreateMovie_AddsAndReturnsCreated()
    {
        var m = new Movies { Title = "New Movie" };

        var result = await _repo.CreateMovie(m);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returned = Assert.IsType<Movies>(created.Value);
        Assert.Equal("New Movie", returned.Title);
        Assert.Equal(1, await _context.Movies.CountAsync());
    }

    [Fact]
    public async Task GetMovie_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.GetMovie(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteMovie_RemovesItem_WhenExists()
    {
        var m = new Movies { Title = "ToDelete" };
        _context.Movies.Add(m);
        await _context.SaveChangesAsync();

        var result = await _repo.DeleteMovie(m.MovieId);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_context.Movies);
    }

    [Fact]
    public async Task GetTotalCountAsync_ReturnsCount()
    {
        _context.Movies.AddRange(
            new Movies { Title = "A" },
            new Movies { Title = "B" }
        );
        await _context.SaveChangesAsync();

        var qp = new QueryParams { Page = 1, PageSize = 10 };

        var count = await _repo.GetTotalCountAsync(qp);

        Assert.Equal(2, count);
    }
}

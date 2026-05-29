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

public class AnimesRepoTests
{
    private readonly MyDBContext _context;
    private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
    private readonly AnimesRepo _repo;

    public AnimesRepoTests()
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

        _repo = new AnimesRepo(_context, _hubContextMock.Object);
    }

    [Fact]
    public async Task GetAnimes_ReturnsAnimesFromContext()
    {
        _context.Animes.AddRange(
            new Anime { Title = "Anime A" },
            new Anime { Title = "Anime B" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var result = await _repo.GetAnimes(queryParams);

        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetAnime_WithExistingId_ReturnsAnime()
    {
        var anime = new Anime { Title = "My Anime" };
        _context.Animes.Add(anime);
        await _context.SaveChangesAsync();

        var result = await _repo.GetAnime(anime.AnimeId);

        Assert.NotNull(result.Value);
        Assert.Equal(anime.AnimeId, result.Value!.AnimeId);
        Assert.Equal("My Anime", result.Value.Title);
    }

    [Fact]
    public async Task GetAnime_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.GetAnime(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAnime_AddsAnimeAndReturnsCreatedResult()
    {
        var anime = new Anime { Title = "New Anime" };

        var result = await _repo.CreateAnime(anime);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedAnime = Assert.IsType<Anime>(createdResult.Value);
        Assert.Equal("New Anime", returnedAnime.Title);
        Assert.Equal(1, await _context.Animes.CountAsync());
        Assert.Equal(anime.AnimeId, returnedAnime.AnimeId);
    }

    [Fact]
    public async Task PutAnime_WithMismatchedId_ReturnsBadRequest()
    {
        var anime = new Anime { Title = "Existing Anime" };
        _context.Animes.Add(anime);
        await _context.SaveChangesAsync();

        var updatedAnime = new Anime { AnimeId = anime.AnimeId + 1, Title = "Updated Anime" };

        var result = await _repo.PutAnime(anime.AnimeId, updatedAnime);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteAnime_RemovesAnime_WhenExists()
    {
        var anime = new Anime { Title = "Delete Anime" };
        _context.Animes.Add(anime);
        await _context.SaveChangesAsync();

        var result = await _repo.DeleteAnime(anime.AnimeId);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_context.Animes);
    }

    [Fact]
    public async Task DeleteAnime_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.DeleteAnime(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTotalCountAsync_ReturnsTotalCount()
    {
        _context.Animes.AddRange(
            new Anime { Title = "A" },
            new Anime { Title = "B" },
            new Anime { Title = "C" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var count = await _repo.GetTotalCountAsync(queryParams);

        Assert.Equal(3, count);
    }
}

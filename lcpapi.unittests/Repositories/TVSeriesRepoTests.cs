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

public class TVSeriesRepoTests
{
    private readonly MyDBContext _context;
    private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
    private readonly TvseriesRepo _repo;

    public TVSeriesRepoTests()
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

        _repo = new TvseriesRepo(_context, _hubContextMock.Object);
    }

    [Fact]
    public async Task GetTvseries_ReturnsTvseriesFromContext()
    {
        _context.Tvseries.AddRange(
            new Tvseries { Title = "Series A" },
            new Tvseries { Title = "Series B" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var result = await _repo.GetTvseries(queryParams);

        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetTvserie_WithExistingId_ReturnsTvserie()
    {
        var tvserie = new Tvseries { Title = "My Series" };
        _context.Tvseries.Add(tvserie);
        await _context.SaveChangesAsync();

        var result = await _repo.GetTvserie(tvserie.TvserieId);

        Assert.NotNull(result.Value);
        Assert.Equal(tvserie.TvserieId, result.Value!.TvserieId);
        Assert.Equal("My Series", result.Value.Title);
    }

    [Fact]
    public async Task GetTvserie_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.GetTvserie(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateTvserie_AddsTvserieAndReturnsCreatedResult()
    {
        var tvserie = new Tvseries { Title = "New Series" };

        var result = await _repo.CreateTvserie(tvserie);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedTvserie = Assert.IsType<Tvseries>(createdResult.Value);
        Assert.Equal("New Series", returnedTvserie.Title);
        Assert.Equal(1, await _context.Tvseries.CountAsync());
        Assert.Equal(tvserie.TvserieId, returnedTvserie.TvserieId);
    }

    [Fact]
    public async Task PutTvserie_WithMismatchedId_ReturnsBadRequest()
    {
        var tvserie = new Tvseries { Title = "Existing Series" };
        _context.Tvseries.Add(tvserie);
        await _context.SaveChangesAsync();

        var updatedTvserie = new Tvseries { TvserieId = tvserie.TvserieId + 1, Title = "Updated Series" };

        var result = await _repo.PutTvserie(tvserie.TvserieId, updatedTvserie);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteTvserie_RemovesTvserie_WhenExists()
    {
        var tvserie = new Tvseries { Title = "Delete Series" };
        _context.Tvseries.Add(tvserie);
        await _context.SaveChangesAsync();

        var result = await _repo.DeleteTvserie(tvserie.TvserieId);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_context.Tvseries);
    }

    [Fact]
    public async Task DeleteAllTvserie_RemovesAllTvseries_WhenExists()
    {
        _context.Tvseries.AddRange(
            new Tvseries { Title = "Series 1" },
            new Tvseries { Title = "Series 2" }
        );
        await _context.SaveChangesAsync();

        var result = await _repo.DeleteAllTvserie();

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_context.Tvseries);
    }

    [Fact]
    public async Task DeleteAllTvserie_ReturnsNotFound_WhenNoTvseriesExist()
    {
        var result = await _repo.DeleteAllTvserie();

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTvseriesAllInfo_ReturnsOkWithAggregatedData()
    {
        var tvserie = new Tvseries { Title = "Series Info" };
        _context.Tvseries.Add(tvserie);
        await _context.SaveChangesAsync();

        _context.TvseriesSeasonsInfo.Add(new TvseriesSeasonsInfo { SeasonsTitle = "Season 1", TvserieId = tvserie.TvserieId });
        _context.TvseriesEpisodesInfos.Add(new TvseriesEpisodesInfo { EpisodesTitle = "Episode 1", TvserieId = tvserie.TvserieId });
        _context.TvseriesReviewsInfos.Add(new TvseriesReviewsInfo { ReviewsTitle = "Review 1", ReviewsDescription = "Great", TvserieId = tvserie.TvserieId });
        await _context.SaveChangesAsync();

        var result = await _repo.GetTvseriesAllInfo();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetTvseriesAllInfoById_WithExistingId_ReturnsOk()
    {
        var tvserie = new Tvseries { Title = "Series Detail" };
        _context.Tvseries.Add(tvserie);
        await _context.SaveChangesAsync();

        _context.TvseriesSeasonsInfo.Add(new TvseriesSeasonsInfo { SeasonsTitle = "Season A", TvserieId = tvserie.TvserieId });
        _context.TvseriesEpisodesInfos.Add(new TvseriesEpisodesInfo { EpisodesTitle = "Episode A", TvserieId = tvserie.TvserieId });
        _context.TvseriesReviewsInfos.Add(new TvseriesReviewsInfo { ReviewsTitle = "Review A", ReviewsDescription = "Nice", TvserieId = tvserie.TvserieId });
        await _context.SaveChangesAsync();

        var result = await _repo.GetTvseriesAllInfoById(tvserie.TvserieId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetTvseriesAllInfoById_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.GetTvseriesAllInfoById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTotalCountAsync_ReturnsTotalCount()
    {
        _context.Tvseries.AddRange(
            new Tvseries { Title = "A" },
            new Tvseries { Title = "B" },
            new Tvseries { Title = "C" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var count = await _repo.GetTotalCountAsync(queryParams);

        Assert.Equal(3, count);
    }
}

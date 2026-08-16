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

public class RecipesFoodsRepoTests
{
    private readonly MyDBContext _context;
    private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
    private readonly RecipesFoodsRepo _repo;

    public RecipesFoodsRepoTests()
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

        _repo = new RecipesFoodsRepo(_context, _hubContextMock.Object);
    }

    [Fact]
    public async Task GetRecipesFoods_ReturnsItems()
    {
        _context.RecipesFoods.AddRange(
            new RecipesFoods { Title = "R1" },
            new RecipesFoods { Title = "R2" }
        );
        await _context.SaveChangesAsync();

        var qp = new QueryParams { Page = 1, PageSize = 10 };

        var result = await _repo.GetRecipesFoods(qp);

        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task CreateRecipesFoods_AddsAndReturnsCreated()
    {
        var r = new RecipesFoods { Title = "New Recipe" };

        var result = await _repo.CreateRecipesFoods(r);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returned = Assert.IsType<RecipesFoods>(created.Value);
        Assert.Equal("New Recipe", returned.Title);
        Assert.Equal(1, await _context.RecipesFoods.CountAsync());
    }

    [Fact]
    public async Task DeleteRecipesFoods_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.DeleteRecipesFoods(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTotalCountAsync_ReturnsCount()
    {
        _context.RecipesFoods.AddRange(
            new RecipesFoods { Title = "A" },
            new RecipesFoods { Title = "B" }
        );
        await _context.SaveChangesAsync();

        var qp = new QueryParams { Page = 1, PageSize = 10 };

        var count = await _repo.GetTotalCountAsync(qp);

        Assert.Equal(2, count);
    }
}

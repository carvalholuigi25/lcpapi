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

public class ActionFiguresRepoTests
{
    private readonly MyDBContext _context;
    private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
    private readonly ActionFiguresRepo _repo;

    public ActionFiguresRepoTests()
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

        _repo = new ActionFiguresRepo(_context, _hubContextMock.Object);
    }

    [Fact]
    public async Task GetActionFigures_ReturnsItemsFromContext()
    {
        _context.ActionFigures.AddRange(
            new ActionFigure { Name = "A1" },
            new ActionFigure { Name = "A2" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var result = await _repo.GetActionFigures(queryParams);

        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetActionFigure_WithExistingId_ReturnsItem()
    {
        var item = new ActionFigure { Name = "X" };
        _context.ActionFigures.Add(item);
        await _context.SaveChangesAsync();

        var result = await _repo.GetActionFigure(item.ActionFigureId);

        Assert.NotNull(result.Value);
        Assert.Equal(item.ActionFigureId, result.Value!.ActionFigureId);
    }

    [Fact]
    public async Task CreateActionFigure_AddsAndReturnsCreated()
    {
        var item = new ActionFigure { Name = "New" };

        var result = await _repo.CreateActionFigure(item);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returned = Assert.IsType<ActionFigure>(created.Value);
        Assert.Equal("New", returned.Name);
        Assert.Equal(1, await _context.ActionFigures.CountAsync());
    }

    [Fact]
    public async Task PutActionFigure_WithMismatchedId_ReturnsBadRequest()
    {
        var item = new ActionFigure { Name = "Old" };
        _context.ActionFigures.Add(item);
        await _context.SaveChangesAsync();

        var updated = new ActionFigure { ActionFigureId = item.ActionFigureId + 1, Name = "Updated" };

        var result = await _repo.PutActionFigure(item.ActionFigureId, updated);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteActionFigure_RemovesItem_WhenExists()
    {
        var item = new ActionFigure { Name = "Del" };
        _context.ActionFigures.Add(item);
        await _context.SaveChangesAsync();

        var result = await _repo.DeleteActionFigure(item.ActionFigureId);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_context.ActionFigures);
    }

    [Fact]
    public async Task GetTotalCountAsync_ReturnsCount()
    {
        _context.ActionFigures.AddRange(
            new ActionFigure { Name = "1" },
            new ActionFigure { Name = "2" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var count = await _repo.GetTotalCountAsync(queryParams);

        Assert.Equal(2, count);
    }
}

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

public class SoftwaresRepoTests
{
    private readonly MyDBContext _context;
    private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
    private readonly SoftwaresRepo _repo;

    public SoftwaresRepoTests()
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

        _repo = new SoftwaresRepo(_context, _hubContextMock.Object);
    }

    [Fact]
    public async Task GetSoftwares_ReturnsSoftwaresFromContext()
    {
        _context.Softwares.AddRange(
            new Software { Name = "Software A" },
            new Software { Name = "Software B" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var result = await _repo.GetSoftwares(queryParams);

        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetSoftware_WithExistingId_ReturnsSoftware()
    {
        var software = new Software { Name = "My Software" };
        _context.Softwares.Add(software);
        await _context.SaveChangesAsync();

        var result = await _repo.GetSoftware(software.SoftwareId);

        Assert.NotNull(result.Value);
        Assert.Equal(software.SoftwareId, result.Value!.SoftwareId);
        Assert.Equal("My Software", result.Value.Name);
    }

    [Fact]
    public async Task GetSoftware_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.GetSoftware(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateSoftware_AddsSoftwareAndReturnsCreatedResult()
    {
        var software = new Software { Name = "New Software" };

        var result = await _repo.CreateSoftware(software);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedSoftware = Assert.IsType<Software>(createdResult.Value);
        Assert.Equal("New Software", returnedSoftware.Name);
        Assert.Equal(1, await _context.Softwares.CountAsync());
        Assert.Equal(software.SoftwareId, returnedSoftware.SoftwareId);
    }

    [Fact]
    public async Task PutSoftware_WithMismatchedId_ReturnsBadRequest()
    {
        var software = new Software { Name = "Existing Software" };
        _context.Softwares.Add(software);
        await _context.SaveChangesAsync();

        var updatedSoftware = new Software { SoftwareId = software.SoftwareId + 1, Name = "Updated Software" };

        var result = await _repo.PutSoftware(software.SoftwareId, updatedSoftware);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteSoftware_RemovesSoftware_WhenExists()
    {
        var software = new Software { Name = "Delete Software" };
        _context.Softwares.Add(software);
        await _context.SaveChangesAsync();

        var result = await _repo.DeleteSoftware(software.SoftwareId);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_context.Softwares);
    }

    [Fact]
    public async Task DeleteSoftware_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.DeleteSoftware(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTotalCountAsync_ReturnsTotalCount()
    {
        _context.Softwares.AddRange(
            new Software { Name = "A" },
            new Software { Name = "B" },
            new Software { Name = "C" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var count = await _repo.GetTotalCountAsync(queryParams);

        Assert.Equal(3, count);
    }
}

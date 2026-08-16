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

public class PetsRepoTests
{
    private readonly MyDBContext _context;
    private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
    private readonly PetRepo _repo;

    public PetsRepoTests()
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

        _repo = new PetRepo(_context, _hubContextMock.Object);
    }

    [Fact]
    public async Task GetPets_ReturnsPetsFromContext()
    {
        _context.Pets.AddRange(
            new Pet { Name = "Pet A", Image = "img-a.png", Type = "Dog" },
            new Pet { Name = "Pet B", Image = "img-b.png", Type = "Cat" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var result = await _repo.GetPets(queryParams);

        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetPet_WithExistingId_ReturnsPet()
    {
        var pet = new Pet { Name = "My Pet", Image = "img.png", Type = "Cat" };
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        var result = await _repo.GetPet(pet.PetsId);

        Assert.NotNull(result.Value);
        Assert.Equal(pet.PetsId, result.Value!.PetsId);
        Assert.Equal("My Pet", result.Value.Name);
    }

    [Fact]
    public async Task GetPet_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.GetPet(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreatePet_AddsPetAndReturnsCreatedResult()
    {
        var pet = new Pet { Name = "New Pet", Image = "img-new.png", Type = "Bird" };

        var result = await _repo.CreatePet(pet);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedPet = Assert.IsType<Pet>(createdResult.Value);
        Assert.Equal("New Pet", returnedPet.Name);
        Assert.Equal(1, await _context.Pets.CountAsync());
        Assert.Equal(pet.PetsId, returnedPet.PetsId);
    }

    [Fact]
    public async Task PutPet_WithMismatchedId_ReturnsBadRequest()
    {
        var pet = new Pet { Name = "Existing Pet", Image = "img.png", Type = "Dog" };
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        var updatedPet = new Pet { PetsId = pet.PetsId + 1, Name = "Updated Pet", Image = "img-updated.png", Type = "Dog" };

        var result = await _repo.PutPet(pet.PetsId, updatedPet);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeletePet_RemovesPet_WhenExists()
    {
        var pet = new Pet { Name = "Delete Pet", Image = "img-delete.png", Type = "Fish" };
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        var result = await _repo.DeletePet(pet.PetsId);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_context.Pets);
    }

    [Fact]
    public async Task DeletePet_WithMissingId_ReturnsNotFound()
    {
        var result = await _repo.DeletePet(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTotalCountAsync_ReturnsTotalCount()
    {
        _context.Pets.AddRange(
            new Pet { Name = "A", Image = "img-a.png", Type = "Cat" },
            new Pet { Name = "B", Image = "img-b.png", Type = "Dog" },
            new Pet { Name = "C", Image = "img-c.png", Type = "Parrot" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        var count = await _repo.GetTotalCountAsync(queryParams);

        Assert.Equal(3, count);
    }
}

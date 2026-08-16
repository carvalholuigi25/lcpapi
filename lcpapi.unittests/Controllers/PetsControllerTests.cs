using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using lcpapi.Controllers;
using lcpapi.Interfaces;
using lcpapi.Models;
using lcpapi.Models.QParams;
using Xunit;

namespace lcpapi.unittests.Controllers;

/// <summary>
/// Unit tests for PetController.
/// </summary>
public class PetsControllerTests
{
    /// <summary>
    /// Tests the GetPet method of PetController to ensure it returns an OkResult with a QueryParamsResp containing the expected data.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetPets_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var Pets = new List<Pet>
        {
            new Pet { PetsId = 1, Name = "Teco" },
            new Pet { PetsId = 2, Name = "Riscas" },
            new Pet { PetsId = 3, Name = "Manchas" },
        };

        var mockRepo = new Mock<IPetsRepo>();
        mockRepo.Setup(r => r.GetPets(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<Pet>>(Pets));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(Pets.Count);

        var controller = new PetController(mockRepo.Object);

        var result = await controller.GetPet(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<Pet>>(okResult.Value);

        Assert.Equal(Pets.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Equal(3, response.Data.Count);
        Assert.Equal("Teco", response.Data[0].Name);
        Assert.Equal("Riscas", response.Data[1].Name);
        Assert.Equal("Manchas", response.Data[2].Name);
    }

    /// <summary>
    /// Tests the GetPet method of PetController to ensure it returns an OkResult with the expected Pet data when a valid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetPet_ById_ReturnsOkResult_WithQueryParamsResp()
    {
        int id = 1;
        var Pet = new Pet { PetsId = id, Name = "Teco" };

        var mockRepo = new Mock<IPetsRepo>();
        mockRepo.Setup(r => r.GetPet(id))
                .ReturnsAsync(new ActionResult<Pet>(Pet));

        var controller = new PetController(mockRepo.Object);

        var result = await controller.GetPet(id);

        var response = Assert.IsType<Pet>(result.Value);

        Assert.Equal(Pet.PetsId, response.PetsId);
        Assert.Equal(Pet.Name, response.Name);
    }

    /// <summary>
    /// Tests the GetPet method of PetController to ensure it returns a NotFoundResult when an invalid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetPet_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IPetsRepo>();
        mockRepo.Setup(r => r.GetPet(id))
                .ReturnsAsync(new ActionResult<Pet>(new NotFoundResult()));

        var controller = new PetController(mockRepo.Object);

        var result = await controller.GetPet(id);

        Assert.IsType<NotFoundResult>(result.Result);   
    }

    /// <summary>
    /// Tests the CreatePet method of PetController to ensure it returns a CreatedAtActionResult with the expected Pet data when a new pet is created successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreatePet_ReturnsCreatedAtActionResult()
    {
        var newPet = new Pet { Name = "Teco" };
        var createdPet = new Pet { PetsId = 1, Name = "Teco" };

        var mockRepo = new Mock<IPetsRepo>();
        mockRepo.Setup(r => r.CreatePet(newPet))
                .ReturnsAsync(new ActionResult<Pet>(new CreatedAtActionResult(nameof(PetController.GetPet), "Pet", new { id = createdPet.PetsId }, createdPet)));

        var controller = new PetController(mockRepo.Object);

        var result = await controller.CreatePet(newPet);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<Pet>(createdAtActionResult.Value);

        Assert.Equal(createdPet.PetsId, response.PetsId);
        Assert.Equal(createdPet.Name, response.Name);
    }

    /// <summary>
    ///    Tests the PutPet method of PetController to ensure it returns an OkResult when an existing pet is updated successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutPet_ById_ReturnsOkResult()
    {
        int id = 1;
        var Pet = new Pet { PetsId = id, Name = "Teco" };

        var mockRepo = new Mock<IPetsRepo>();
        mockRepo.Setup(r => r.PutPet(id, Pet))
                .ReturnsAsync(new OkResult());

        var controller = new PetController(mockRepo.Object);

        var result = await controller.PutPet(id, Pet);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///    Tests the PutPet method of PetController to ensure it returns a NotFoundResult when trying to update a non-existing pet.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutPet_ById_ReturnsNotFoundResult()
    {
        int id = 1;
        var Pet = new Pet { PetsId = id, Name = "Teco" };

        var mockRepo = new Mock<IPetsRepo>();
        mockRepo.Setup(r => r.PutPet(id, Pet))
                .ReturnsAsync(new NotFoundResult());

        var controller = new PetController(mockRepo.Object);

        var result = await controller.PutPet(id, Pet);

        Assert.IsType<NotFoundResult>(result);
    }

     /// <summary>
     /// Tests the DeletePet method of PetController to ensure it returns an OkResult when an existing pet is deleted successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeletePet_ById_ReturnsOkResult()
    {
        int id = 1;

        var mockRepo = new Mock<IPetsRepo>();
        mockRepo.Setup(r => r.DeletePet(id))
                .ReturnsAsync(new OkResult());

        var controller = new PetController(mockRepo.Object);

        var result = await controller.DeletePet(id);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///   Tests the DeletePet method of PetController to ensure it returns a NotFoundResult when trying to delete a non-existing pet.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeletePet_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IPetsRepo>();
        mockRepo.Setup(r => r.DeletePet(id))
                .ReturnsAsync(new NotFoundResult());

        var controller = new PetController(mockRepo.Object);

        var result = await controller.DeletePet(id);

        Assert.IsType<NotFoundResult>(result);
    }
}

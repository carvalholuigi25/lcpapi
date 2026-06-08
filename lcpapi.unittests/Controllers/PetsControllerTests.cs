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

public class PetsControllerTests
{
    [Fact]
    public async Task GetPet_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var pets = new List<Pet>
        {
            new Pet { PetsId = 1, Name = "Teco" },
            new Pet { PetsId = 2, Name = "Riscas" },
            new Pet { PetsId = 3, Name = "Manchas"}
        };

        var mockRepo = new Mock<IPetsRepo>();
        mockRepo.Setup(r => r.GetPets(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<Pet>>(pets));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(pets.Count);

        var controller = new PetController(mockRepo.Object);

        var result = await controller.GetPet(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<Pet>>(okResult.Value);

        Assert.Equal(pets.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Equal(3, response.Data.Count);
        Assert.Equal("Teco", response.Data[0].Name);
        Assert.Equal("Riscas", response.Data[1].Name);
        Assert.Equal("Manchas", response.Data[2].Name);
    }
}
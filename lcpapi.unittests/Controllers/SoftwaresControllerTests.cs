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
/// Unit tests for SoftwaresController.
/// </summary>
public class SoftwaresControllerTests
{

    /// <summary>
    /// Tests the GetSoftwares method of SoftwaresController to ensure it returns an OkResult with a QueryParamsResp containing the expected data.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetSoftwares_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var Softwares = new List<Software>
        {
            new Software { SoftwareId = 1, Name = "LCPWebApp" },
        };

        var mockRepo = new Mock<ISoftwaresRepo>();
        mockRepo.Setup(r => r.GetSoftwares(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<Software>>(Softwares));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(Softwares.Count);

        var controller = new SoftwaresController(mockRepo.Object);

        var result = await controller.GetSoftwares(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<Software>>(okResult.Value);

        Assert.Equal(Softwares.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal("LCPWebApp", response.Data[0].Name);
    }

    /// <summary>
    /// Tests the GetSoftware method of SoftwaresController to ensure it returns an OkResult with the expected Softwares data when a valid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetSoftware_ById_ReturnsOkResult_WithQueryParamsResp()
    {
        int id = 1;
        var Softwares = new Software { SoftwareId = id, Name = "LCPWebApp" };

        var mockRepo = new Mock<ISoftwaresRepo>();
        mockRepo.Setup(r => r.GetSoftware(id))
                .ReturnsAsync(new ActionResult<Software>(Softwares));

        var controller = new SoftwaresController(mockRepo.Object);

        var result = await controller.GetSoftware(id);

        var response = Assert.IsType<Software>(result.Value);

        Assert.Equal(Softwares.SoftwareId, response.SoftwareId);
        Assert.Equal(Softwares.Name, response.Name);
    }

    /// <summary>
    ///     Tests the GetSoftware method of SoftwaresController to ensure it returns a NotFoundResult when an invalid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetSoftware_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<ISoftwaresRepo>();
        mockRepo.Setup(r => r.GetSoftware(id))
                .ReturnsAsync(new ActionResult<Software>(new NotFoundResult()));

        var controller = new SoftwaresController(mockRepo.Object);

        var result = await controller.GetSoftware(id);

        Assert.IsType<NotFoundResult>(result.Result);   
    }

    /// <summary>
    /// Tests the CreateSoftware method of SoftwaresController to ensure it returns a CreatedAtActionResult with the expected Softwares data when a new Software is created successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateSoftware_ReturnsCreatedAtActionResult()
    {
        var newSoftware = new Software { Name = "LCPWebApp" };
        var createdSoftware = new Software { SoftwareId = 1, Name = "LCPWebApp" };

        var mockRepo = new Mock<ISoftwaresRepo>();
        mockRepo.Setup(r => r.CreateSoftware(newSoftware))
                .ReturnsAsync(new ActionResult<Software>(new CreatedAtActionResult(nameof(SoftwaresController.GetSoftware), "Softwares", new { id = createdSoftware.SoftwareId }, createdSoftware)));

        var controller = new SoftwaresController(mockRepo.Object);

        var result = await controller.CreateSoftware(newSoftware);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<Software>(createdAtActionResult.Value);

        Assert.Equal(createdSoftware.SoftwareId, response.SoftwareId);
        Assert.Equal(createdSoftware.Name, response.Name);
    }


    /// <summary>
    /// Tests the PutSoftware method of SoftwaresController to ensure it returns a NoContentResult when an existing Software is updated successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutSoftware_ById_ReturnsOkResult()
    {
        int id = 1;
        var Software = new Software { SoftwareId = id, Name = "LCPWebApp" };

        var mockRepo = new Mock<ISoftwaresRepo>();
        mockRepo.Setup(r => r.PutSoftware(id, Software))
                .ReturnsAsync(new OkResult());

        var controller = new SoftwaresController(mockRepo.Object);

        var result = await controller.PutSoftware(id, Software);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///     Tests the PutSoftware method of SoftwaresController to ensure it returns a NotFoundResult when trying to update a non-existing Software.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutSoftware_ById_ReturnsNotFoundResult()
    {
        int id = 1;
        var Software = new Software { SoftwareId = id, Name = "LCPWebApp" };

        var mockRepo = new Mock<ISoftwaresRepo>();
        mockRepo.Setup(r => r.PutSoftware(id, Software))
                .ReturnsAsync(new NotFoundResult());

        var controller = new SoftwaresController(mockRepo.Object);

        var result = await controller.PutSoftware(id, Software);

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the DeleteSoftware method of SoftwaresController to ensure it returns an OkResult when an existing Software is deleted successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteSoftware_ById_ReturnsOkResult()
    {
        int id = 1;

        var mockRepo = new Mock<ISoftwaresRepo>();
        mockRepo.Setup(r => r.DeleteSoftware(id))
                .ReturnsAsync(new OkResult());

        var controller = new SoftwaresController(mockRepo.Object);

        var result = await controller.DeleteSoftware(id);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///    Tests the DeleteSoftware method of SoftwaresController to ensure it returns a NotFoundResult when trying to delete a non-existing Software.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteSoftware_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<ISoftwaresRepo>();
        mockRepo.Setup(r => r.DeleteSoftware(id))
                .ReturnsAsync(new NotFoundResult());

        var controller = new SoftwaresController(mockRepo.Object);

        var result = await controller.DeleteSoftware(id);

        Assert.IsType<NotFoundResult>(result);
    }
}

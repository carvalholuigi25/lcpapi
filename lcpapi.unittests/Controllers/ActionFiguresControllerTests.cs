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
/// Unit tests for ActionFiguresController.
/// </summary>
public class ActionFiguresControllerTests
{

    /// <summary>
    /// Tests the GetActionFigures method of ActionFiguresController to ensure it returns an OkResult with a QueryParamsResp containing the expected data.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetActionFigures_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var ActionFigures = new List<ActionFigure>
        {
            new ActionFigure { ActionFigureId = 1, Name = "Goku Ultra Instinct (Dragon Ball Super)" },
        };

        var mockRepo = new Mock<IActionFiguresRepo>();
        mockRepo.Setup(r => r.GetActionFigures(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<ActionFigure>>(ActionFigures));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(ActionFigures.Count);

        var controller = new ActionFiguresController(mockRepo.Object);

        var result = await controller.GetActionFigures(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<ActionFigure>>(okResult.Value);

        Assert.Equal(ActionFigures.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal("Goku Ultra Instinct (Dragon Ball Super)", response.Data[0].Name);
    }

    /// <summary>
    /// Tests the GetActionFigure method of ActionFiguresController to ensure it returns an OkResult with the expected ActionFigures data when a valid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetActionFigure_ById_ReturnsOkResult_WithQueryParamsResp()
    {
        int id = 1;
        var ActionFigures = new ActionFigure { ActionFigureId = id, Name = "Goku Ultra Instinct (Dragon Ball Super)" };

        var mockRepo = new Mock<IActionFiguresRepo>();
        mockRepo.Setup(r => r.GetActionFigure(id))
                .ReturnsAsync(new ActionResult<ActionFigure>(ActionFigures));

        var controller = new ActionFiguresController(mockRepo.Object);

        var result = await controller.GetActionFigure(id);

        var response = Assert.IsType<ActionFigure>(result.Value);

        Assert.Equal(ActionFigures.ActionFigureId, response.ActionFigureId);
        Assert.Equal(ActionFigures.Name, response.Name);
    }

    /// <summary>
    ///     Tests the GetActionFigure method of ActionFiguresController to ensure it returns a NotFoundResult when an invalid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetActionFigure_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IActionFiguresRepo>();
        mockRepo.Setup(r => r.GetActionFigure(id))
                .ReturnsAsync(new ActionResult<ActionFigure>(new NotFoundResult()));

        var controller = new ActionFiguresController(mockRepo.Object);

        var result = await controller.GetActionFigure(id);

        Assert.IsType<NotFoundResult>(result.Result);   
    }

    /// <summary>
    /// Tests the CreateActionFigure method of ActionFiguresController to ensure it returns a CreatedAtActionResult with the expected ActionFigures data when a new ActionFigure is created successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateActionFigure_ReturnsCreatedAtActionResult()
    {
        var newActionFigure = new ActionFigure { Name = "Goku Ultra Instinct (Dragon Ball Super)" };
        var createdActionFigure = new ActionFigure { ActionFigureId = 1, Name = "Goku Ultra Instinct (Dragon Ball Super)" };

        var mockRepo = new Mock<IActionFiguresRepo>();
        mockRepo.Setup(r => r.CreateActionFigure(newActionFigure))
                .ReturnsAsync(new ActionResult<ActionFigure>(new CreatedAtActionResult(nameof(ActionFiguresController.GetActionFigure), "ActionFigures", new { id = createdActionFigure.ActionFigureId }, createdActionFigure)));

        var controller = new ActionFiguresController(mockRepo.Object);

        var result = await controller.CreateActionFigure(newActionFigure);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ActionFigure>(createdAtActionResult.Value);

        Assert.Equal(createdActionFigure.ActionFigureId, response.ActionFigureId);
        Assert.Equal(createdActionFigure.Name, response.Name);
    }


    /// <summary>
    /// Tests the PutActionFigure method of ActionFiguresController to ensure it returns a NoContentResult when an existing ActionFigure is updated successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutActionFigure_ById_ReturnsOkResult()
    {
        int id = 1;
        var ActionFigure = new ActionFigure { ActionFigureId = id, Name = "Goku Ultra Instinct (Dragon Ball Super)" };

        var mockRepo = new Mock<IActionFiguresRepo>();
        mockRepo.Setup(r => r.PutActionFigure(id, ActionFigure))
                .ReturnsAsync(new OkResult());

        var controller = new ActionFiguresController(mockRepo.Object);

        var result = await controller.PutActionFigure(id, ActionFigure);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///     Tests the PutActionFigure method of ActionFiguresController to ensure it returns a NotFoundResult when trying to update a non-existing ActionFigure.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutActionFigure_ById_ReturnsNotFoundResult()
    {
        int id = 1;
        var ActionFigure = new ActionFigure { ActionFigureId = id, Name = "Goku Ultra Instinct (Dragon Ball Super)" };

        var mockRepo = new Mock<IActionFiguresRepo>();
        mockRepo.Setup(r => r.PutActionFigure(id, ActionFigure))
                .ReturnsAsync(new NotFoundResult());

        var controller = new ActionFiguresController(mockRepo.Object);

        var result = await controller.PutActionFigure(id, ActionFigure);

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the DeleteActionFigure method of ActionFiguresController to ensure it returns an OkResult when an existing ActionFigure is deleted successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteActionFigure_ById_ReturnsOkResult()
    {
        int id = 1;

        var mockRepo = new Mock<IActionFiguresRepo>();
        mockRepo.Setup(r => r.DeleteActionFigure(id))
                .ReturnsAsync(new OkResult());

        var controller = new ActionFiguresController(mockRepo.Object);

        var result = await controller.DeleteActionFigure(id);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///    Tests the DeleteActionFigure method of ActionFiguresController to ensure it returns a NotFoundResult when trying to delete a non-existing ActionFigure.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteActionFigure_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IActionFiguresRepo>();
        mockRepo.Setup(r => r.DeleteActionFigure(id))
                .ReturnsAsync(new NotFoundResult());

        var controller = new ActionFiguresController(mockRepo.Object);

        var result = await controller.DeleteActionFigure(id);

        Assert.IsType<NotFoundResult>(result);
    }
}

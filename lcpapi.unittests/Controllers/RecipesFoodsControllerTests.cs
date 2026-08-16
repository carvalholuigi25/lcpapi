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
/// Unit tests for RecipesFoodsController.
/// </summary>
public class RecipesFoodsControllerTests
{

    /// <summary>
    /// Tests the GetRecipesFoods method of RecipesFoodsController to ensure it returns an OkResult with a QueryParamsResp containing the expected data.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetRecipesFoods_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var RecipesFoods = new List<RecipesFoods>
        {
            new RecipesFoods { RecipesFoodsId = 1, Title = "Lasanha" },
            new RecipesFoods { RecipesFoodsId = 2, Title = "Arroz Branco" },
            new RecipesFoods { RecipesFoodsId = 3, Title = "Massa Esparguete" },
            new RecipesFoods { RecipesFoodsId = 4, Title = "Francesinha" },
            new RecipesFoods { RecipesFoodsId = 5, Title = "Pizza" },
            new RecipesFoods { RecipesFoodsId = 6, Title = "Double Cheeseburger" },
        };

        var mockRepo = new Mock<IRecipesFoodsRepo>();
        mockRepo.Setup(r => r.GetRecipesFoods(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<RecipesFoods>>(RecipesFoods));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(RecipesFoods.Count);

        var controller = new RecipesFoodsController(mockRepo.Object);

        var result = await controller.GetRecipesFoods(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<RecipesFoods>>(okResult.Value);

        Assert.Equal(RecipesFoods.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Equal(6, response.Data.Count);
        Assert.Equal("Lasanha", response.Data[0].Title);
        Assert.Equal("Arroz Branco", response.Data[1].Title);
        Assert.Equal("Massa Esparguete", response.Data[2].Title);
        Assert.Equal("Francesinha", response.Data[3].Title);
        Assert.Equal("Pizza", response.Data[4].Title);
        Assert.Equal("Double Cheeseburger", response.Data[5].Title);
    }

    /// <summary>
    /// Tests the GetRecipesFoods method of RecipesFoodsController to ensure it returns an OkResult with the expected RecipesFoods data when a valid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetRecipesFoods_ById_ReturnsOkResult_WithQueryParamsResp()
    {
        int id = 1;
        var RecipesFoods = new RecipesFoods { RecipesFoodsId = id, Title = "Lasanha" };

        var mockRepo = new Mock<IRecipesFoodsRepo>();
        mockRepo.Setup(r => r.GetRecipesFood(id))
                .ReturnsAsync(new ActionResult<RecipesFoods>(RecipesFoods));

        var controller = new RecipesFoodsController(mockRepo.Object);

        var result = await controller.GetRecipesFood(id);

        var response = Assert.IsType<RecipesFoods>(result.Value);

        Assert.Equal(RecipesFoods.RecipesFoodsId, response.RecipesFoodsId);
        Assert.Equal(RecipesFoods.Title, response.Title);
    }

    /// <summary>
    ///     Tests the GetRecipesFoods method of RecipesFoodsController to ensure it returns a NotFoundResult when an invalid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetRecipesFoods_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IRecipesFoodsRepo>();
        mockRepo.Setup(r => r.GetRecipesFood(id))
                .ReturnsAsync(new ActionResult<RecipesFoods>(new NotFoundResult()));

        var controller = new RecipesFoodsController(mockRepo.Object);

        var result = await controller.GetRecipesFood(id);

        Assert.IsType<NotFoundResult>(result.Result);   
    }

    /// <summary>
    /// Tests the CreateRecipesFoods method of RecipesFoodsController to ensure it returns a CreatedAtActionResult with the expected RecipesFoods data when a new RecipesFoods is created successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateRecipesFoods_ReturnsCreatedAtActionResult()
    {
        var newRecipesFoods = new RecipesFoods { Title = "Lasanha" };
        var createdRecipesFoods = new RecipesFoods { RecipesFoodsId = 1, Title = "Lasanha" };

        var mockRepo = new Mock<IRecipesFoodsRepo>();
        mockRepo.Setup(r => r.CreateRecipesFoods(newRecipesFoods))
                .ReturnsAsync(new ActionResult<RecipesFoods>(new CreatedAtActionResult(nameof(RecipesFoodsController.GetRecipesFoods), "RecipesFoods", new { id = createdRecipesFoods.RecipesFoodsId }, createdRecipesFoods )));

        var controller = new RecipesFoodsController(mockRepo.Object);

        var result = await controller.CreateRecipesFood(newRecipesFoods);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<RecipesFoods>(createdAtActionResult.Value);

        Assert.Equal(createdRecipesFoods.RecipesFoodsId, response.RecipesFoodsId);
        Assert.Equal(createdRecipesFoods.Title, response.Title);
    }


    /// <summary>
    /// Tests the PutRecipesFoods method of RecipesFoodsController to ensure it returns a NoContentResult when an existing RecipesFoods is updated successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutRecipesFoods_ById_ReturnsOkResult()
    {
        int id = 1;
        var RecipesFoods = new RecipesFoods { RecipesFoodsId = id, Title = "Lasanha" };

        var mockRepo = new Mock<IRecipesFoodsRepo>();
        mockRepo.Setup(r => r.PutRecipesFoods(id, RecipesFoods))
                .ReturnsAsync(new OkResult());

        var controller = new RecipesFoodsController(mockRepo.Object);

        var result = await controller.PutRecipesFood(id, RecipesFoods);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///     Tests the PutRecipesFoods method of RecipesFoodsController to ensure it returns a NotFoundResult when trying to update a non-existing RecipesFoods.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutRecipesFoods_ById_ReturnsNotFoundResult()
    {
        int id = 1;
        var RecipesFoods = new RecipesFoods { RecipesFoodsId = id, Title = "Lasanha" };

        var mockRepo = new Mock<IRecipesFoodsRepo>();
        mockRepo.Setup(r => r.PutRecipesFoods(id, RecipesFoods))
                .ReturnsAsync(new NotFoundResult());

        var controller = new RecipesFoodsController(mockRepo.Object);

        var result = await controller.PutRecipesFood(id, RecipesFoods);

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the DeleteRecipesFoods method of RecipesFoodsController to ensure it returns an OkResult when an existing RecipesFoods is deleted successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteRecipesFoods_ById_ReturnsOkResult()
    {
        int id = 1;

        var mockRepo = new Mock<IRecipesFoodsRepo>();
        mockRepo.Setup(r => r.DeleteRecipesFoods(id))
                .ReturnsAsync(new OkResult());

        var controller = new RecipesFoodsController(mockRepo.Object);

        var result = await controller.DeleteRecipesFood(id);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///    Tests the DeleteRecipesFoods method of RecipesFoodsController to ensure it returns a NotFoundResult when trying to delete a non-existing RecipesFoods.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteRecipesFoods_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IRecipesFoodsRepo>();
        mockRepo.Setup(r => r.DeleteRecipesFoods(id))
                .ReturnsAsync(new NotFoundResult());

        var controller = new RecipesFoodsController(mockRepo.Object);

        var result = await controller.DeleteRecipesFood(id);

        Assert.IsType<NotFoundResult>(result);
    }
}

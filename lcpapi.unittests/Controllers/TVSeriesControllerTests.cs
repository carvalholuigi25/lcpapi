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
/// Unit tests for TvseriesController.
/// </summary>
public class TvseriesControllerTests
{

    /// <summary>
    /// Tests the GetTvseries method of TvseriesController to ensure it returns an OkResult with a QueryParamsResp containing the expected data.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetTvseries_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var Tvseries = new List<Tvseries>
        {
            new Tvseries { TvserieId = 1, Title = "The Flash" },
            new Tvseries { TvserieId = 2, Title = "DC's Legends of Tomorrow" },
            new Tvseries { TvserieId = 3, Title = "Arrow" },
        };

        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.GetTvseries(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<Tvseries>>(Tvseries));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(Tvseries.Count);

        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.GetTvseries(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<Tvseries>>(okResult.Value);

        Assert.Equal(Tvseries.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Equal(3, response.Data.Count);
        Assert.Equal("The Flash", response.Data[0].Title);
        Assert.Equal("DC's Legends of Tomorrow", response.Data[1].Title);
        Assert.Equal("Arrow", response.Data[2].Title);
    }

    /// <summary>
    /// Tests the GetTvserie method of TvseriesController to ensure it returns an OkResult with the expected Tvseries data when a valid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetTvserie_ById_ReturnsOkResult_WithQueryParamsResp()
    {
        int id = 1;
        var Tvseries = new Tvseries { TvserieId = id, Title = "The Flash" };

        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.GetTvserie(id))
                .ReturnsAsync(new ActionResult<Tvseries>(Tvseries));

        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.GetTvserie(id);

        var response = Assert.IsType<Tvseries>(result.Value);

        Assert.Equal(Tvseries.TvserieId, response.TvserieId);
        Assert.Equal(Tvseries.Title, response.Title);
    }

    /// <summary>
    ///     Tests the GetTvserie method of TvseriesController to ensure it returns a NotFoundResult when an invalid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetTvserie_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.GetTvserie(id))
                .ReturnsAsync(new ActionResult<Tvseries>(new NotFoundResult()));

        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.GetTvserie(id);

        Assert.IsType<NotFoundResult>(result.Result);   
    }

    /// <summary>
    /// Tests the GetTvseriesAllInfoS method of TvseriesController to ensure it returns an OkResult with a list of Tvseries when the repository returns data successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetTvseriesAllInfoS_ShouldReturnOkResultWithTvseriesList()
    {
        var TvseriesList = new List<Tvseries>
        {
            new Tvseries { TvserieId = 1, Title = "The Flash" },
            new Tvseries { TvserieId = 2, Title = "DC's Legends of Tomorrow" },
            new Tvseries { TvserieId = 3, Title = "Arrow" },
        };

        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.GetTvseriesAllInfo())
                .ReturnsAsync(new OkObjectResult(TvseriesList));

        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.GetTvseriesAllInfoS();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<Tvseries>>(okResult.Value);

        Assert.Equal(TvseriesList.Count, response.Count);
        Assert.Equal("The Flash", response[0].Title);
        Assert.Equal("DC's Legends of Tomorrow", response[1].Title);
        Assert.Equal("Arrow", response[2].Title);
    }

    [Fact]
    public async Task GetTvseriesAllInfoSById_ShouldReturnOkResultWithTvserie()
    {
        int id = 1;
        var Tvserie = new Tvseries { TvserieId = id, Title = "The Flash" };

        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.GetTvseriesAllInfoById(id))
                .ReturnsAsync(new OkObjectResult(Tvserie));
        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.GetTvseriesAllInfoSById(id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Tvseries>(okResult.Value);

        Assert.Equal(Tvserie.TvserieId, response.TvserieId);
        Assert.Equal(Tvserie.Title, response.Title);
    }

    /// <summary>
    /// Tests the CreateTvserie method of TvseriesController to ensure it returns a CreatedAtActionResult with the expected Tvseries data when a new Tvserie is created successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateTvserie_ReturnsCreatedAtActionResult()
    {
        var newTvserie = new Tvseries { Title = "Avengers: Endgame" };
        var createdTvserie = new Tvseries { TvserieId = 1, Title = "Avengers: Endgame" };

        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.CreateTvserie(newTvserie))
                .ReturnsAsync(new ActionResult<Tvseries>(new CreatedAtActionResult(nameof(TvseriesController.GetTvserie), "Tvseries", new { id = createdTvserie.TvserieId }, createdTvserie)));

        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.CreateTvserie(newTvserie);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<Tvseries>(createdAtActionResult.Value);

        Assert.Equal(createdTvserie.TvserieId, response.TvserieId);
        Assert.Equal(createdTvserie.Title, response.Title);
    }


    /// <summary>
    /// Tests the PutTvserie method of TvseriesController to ensure it returns a NoContentResult when an existing Tvserie is updated successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutTvserie_ById_ReturnsOkResult()
    {
        int id = 1;
        var Tvserie = new Tvseries { TvserieId = id, Title = "Avengers: Infinity War" };

        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.PutTvserie(id, Tvserie))
                .ReturnsAsync(new OkResult());

        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.PutTvserie(id, Tvserie);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///     Tests the PutTvserie method of TvseriesController to ensure it returns a NotFoundResult when trying to update a non-existing Tvserie.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutTvserie_ById_ReturnsNotFoundResult()
    {
        int id = 1;
        var Tvserie = new Tvseries { TvserieId = id, Title = "Avengers: Infinity War" };

        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.PutTvserie(id, Tvserie))
                .ReturnsAsync(new NotFoundResult());

        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.PutTvserie(id, Tvserie);

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the DeleteTvserie method of TvseriesController to ensure it returns an OkResult when an existing Tvserie is deleted successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteTvserie_ById_ReturnsOkResult()
    {
        int id = 1;

        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.DeleteTvserie(id))
                .ReturnsAsync(new OkResult());

        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.DeleteTvserie(id);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///    Tests the DeleteTvserie method of TvseriesController to ensure it returns a NotFoundResult when trying to delete a non-existing Tvserie.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteTvserie_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.DeleteTvserie(id))
                .ReturnsAsync(new NotFoundResult());

        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.DeleteTvserie(id);

        Assert.IsType<NotFoundResult>(result);
    }


    /// <summary>
    /// Tests the DeleteAllTvserie method of TvseriesController to ensure it returns an OkResult when all Tvseries are deleted successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteAllTvserie_ReturnsOkResult()
    {
        var mockRepo = new Mock<ITvseriesRepo>();
        mockRepo.Setup(r => r.DeleteAllTvserie())
                .ReturnsAsync(new OkResult());

        var controller = new TvseriesController(mockRepo.Object);

        var result = await controller.DeleteAllTvserie();

        Assert.IsType<OkResult>(result);
    }
}

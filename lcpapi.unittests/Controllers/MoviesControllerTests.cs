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
/// Unit tests for MoviesController.
/// </summary>
public class MoviesControllerTests
{

    /// <summary>
    /// Tests the GetMovies method of MoviesController to ensure it returns an OkResult with a QueryParamsResp containing the expected data.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMovies_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var Movies = new List<Movies>
        {
            new Movies { MovieId = 1, Title = "Avengers: Endgame" },
        };

        var mockRepo = new Mock<IMoviesRepo>();
        mockRepo.Setup(r => r.GetMovies(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<Movies>>(Movies));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(Movies.Count);

        var controller = new MoviesController(mockRepo.Object);

        var result = await controller.GetMovies(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<Movies>>(okResult.Value);

        Assert.Equal(Movies.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal("Avengers: Endgame", response.Data[0].Title);
    }

    /// <summary>
    /// Tests the GetMovie method of MoviesController to ensure it returns an OkResult with the expected Movies data when a valid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMovie_ById_ReturnsOkResult_WithQueryParamsResp()
    {
        int id = 1;
        var Movies = new Movies { MovieId = id, Title = "Avengers: Endgame" };

        var mockRepo = new Mock<IMoviesRepo>();
        mockRepo.Setup(r => r.GetMovie(id))
                .ReturnsAsync(new ActionResult<Movies>(Movies));

        var controller = new MoviesController(mockRepo.Object);

        var result = await controller.GetMovie(id);

        var response = Assert.IsType<Movies>(result.Value);

        Assert.Equal(Movies.MovieId, response.MovieId);
        Assert.Equal(Movies.Title, response.Title);
    }

    /// <summary>
    ///     Tests the GetMovie method of MoviesController to ensure it returns a NotFoundResult when an invalid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMovie_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IMoviesRepo>();
        mockRepo.Setup(r => r.GetMovie(id))
                .ReturnsAsync(new ActionResult<Movies>(new NotFoundResult()));

        var controller = new MoviesController(mockRepo.Object);

        var result = await controller.GetMovie(id);

        Assert.IsType<NotFoundResult>(result.Result);   
    }

    /// <summary>
    /// Tests the CreateMovie method of MoviesController to ensure it returns a CreatedAtActionResult with the expected Movies data when a new movie is created successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateMovie_ReturnsCreatedAtActionResult()
    {
        var newMovie = new Movies { Title = "Avengers: Endgame" };
        var createdMovie = new Movies { MovieId = 1, Title = "Avengers: Endgame" };

        var mockRepo = new Mock<IMoviesRepo>();
        mockRepo.Setup(r => r.CreateMovie(newMovie))
                .ReturnsAsync(new ActionResult<Movies>(new CreatedAtActionResult(nameof(MoviesController.GetMovie), "Movies", new { id = createdMovie.MovieId }, createdMovie)));

        var controller = new MoviesController(mockRepo.Object);

        var result = await controller.CreateMovie(newMovie);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<Movies>(createdAtActionResult.Value);

        Assert.Equal(createdMovie.MovieId, response.MovieId);
        Assert.Equal(createdMovie.Title, response.Title);
    }


    /// <summary>
    /// Tests the PutMovie method of MoviesController to ensure it returns a NoContentResult when an existing movie is updated successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutMovie_ById_ReturnsOkResult()
    {
        int id = 1;
        var Movie = new Movies { MovieId = id, Title = "Avengers: Infinity War" };

        var mockRepo = new Mock<IMoviesRepo>();
        mockRepo.Setup(r => r.PutMovie(id, Movie))
                .ReturnsAsync(new OkResult());

        var controller = new MoviesController(mockRepo.Object);

        var result = await controller.PutMovie(id, Movie);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///     Tests the PutMovie method of MoviesController to ensure it returns a NotFoundResult when trying to update a non-existing movie.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutMovie_ById_ReturnsNotFoundResult()
    {
        int id = 1;
        var Movie = new Movies { MovieId = id, Title = "Avengers: Infinity War" };

        var mockRepo = new Mock<IMoviesRepo>();
        mockRepo.Setup(r => r.PutMovie(id, Movie))
                .ReturnsAsync(new NotFoundResult());

        var controller = new MoviesController(mockRepo.Object);

        var result = await controller.PutMovie(id, Movie);

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the DeleteMovie method of MoviesController to ensure it returns an OkResult when an existing movie is deleted successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteMovie_ById_ReturnsOkResult()
    {
        int id = 1;

        var mockRepo = new Mock<IMoviesRepo>();
        mockRepo.Setup(r => r.DeleteMovie(id))
                .ReturnsAsync(new OkResult());

        var controller = new MoviesController(mockRepo.Object);

        var result = await controller.DeleteMovie(id);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///    Tests the DeleteMovie method of MoviesController to ensure it returns a NotFoundResult when trying to delete a non-existing movie.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteMovie_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IMoviesRepo>();
        mockRepo.Setup(r => r.DeleteMovie(id))
                .ReturnsAsync(new NotFoundResult());

        var controller = new MoviesController(mockRepo.Object);

        var result = await controller.DeleteMovie(id);

        Assert.IsType<NotFoundResult>(result);
    }
}

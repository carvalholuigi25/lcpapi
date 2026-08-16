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
/// Unit tests for GamesController.
/// </summary>
public class GamesControllerTests
{
    /// <summary>
    /// Tests the GetGame method of GamesController to ensure it returns an OkResult with a QueryParamsResp containing the expected data.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetGames_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var Games = new List<Game>
        {
            new Game { GameId = 1, Title = "Dragon Ball Sparking Zero" },
            new Game { GameId = 2, Title = "007 First Light" },
            new Game { GameId = 3, Title = "Grand Theft Auto V" }
        };

        var mockRepo = new Mock<IGamesRepo>();
        mockRepo.Setup(r => r.GetGames(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<Game>>(Games));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(Games.Count);

        var controller = new GamesController(mockRepo.Object, null!);

        var result = await controller.GetGames(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<Game>>(okResult.Value);

        Assert.Equal(Games.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Equal(3, response.Data.Count);
        Assert.Equal("Dragon Ball Sparking Zero", response.Data[0].Title);
        Assert.Equal("007 First Light", response.Data[1].Title);
        Assert.Equal("Grand Theft Auto V", response.Data[2].Title);
    }

    /// <summary>
    /// Tests the GetGame method of GamesController to ensure it returns an OkResult with the expected Game data when a valid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetGame_ById_ReturnsOkResult_WithQueryParamsResp()
    {
        int id = 1;
        var Game = new Game { GameId = id, Title = "Dragon Ball Sparking Zero" };

        var mockRepo = new Mock<IGamesRepo>();
        mockRepo.Setup(r => r.GetGame(id))
                .ReturnsAsync(new ActionResult<Game>(Game));

        var controller = new GamesController(mockRepo.Object, null!);

        var result = await controller.GetGame(id);

        var response = Assert.IsType<Game>(result.Value);

        Assert.Equal(Game.GameId, response.GameId);
        Assert.Equal(Game.Title, response.Title);
    }

    /// <summary>
    /// Tests the GetGame method of GamesController to ensure it returns a NotFoundResult when an invalid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetGame_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IGamesRepo>();
        mockRepo.Setup(r => r.GetGame(id))
                .ReturnsAsync(new ActionResult<Game>(new NotFoundResult()));

        var controller = new GamesController(mockRepo.Object, null!);

        var result = await controller.GetGame(id);

        Assert.IsType<NotFoundResult>(result.Result);   
    }

    /// <summary>
    /// Tests the CreateGame method of GamesController to ensure it returns a CreatedAtActionResult
    ///     with the expected Game data when a new game is created successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateGame_ReturnsCreatedAtActionResult()
    {
        var newGame = new Game { Title = "Dragon Ball Sparking Zero" };
        var createdGame = new Game { GameId = 1, Title = "Dragon Ball Sparking Zero" };

        var mockRepo = new Mock<IGamesRepo>();
        mockRepo.Setup(r => r.CreateGame(newGame))
                .ReturnsAsync(new ActionResult<Game>(new CreatedAtActionResult(nameof(GamesController.GetGame), "Games", new { id = createdGame.GameId }, createdGame)));

        var controller = new GamesController(mockRepo.Object, null!);

        var result = await controller.CreateGame(newGame);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<Game>(createdAtActionResult.Value);

        Assert.Equal(createdGame.GameId, response.GameId);
        Assert.Equal(createdGame.Title, response.Title);
    }

    /// <summary>
    /// Tests the PutGame method of GamesController to ensure it returns an OkResult when an existing game is updated successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutGame_ById_ReturnsOkResult()
    {
        int id = 1;
        var Game = new Game { GameId = id, Title = "Dragon Ball Z Budokai Tenkaichi 3" };

        var mockRepo = new Mock<IGamesRepo>();
        mockRepo.Setup(r => r.PutGame(id, Game))
                .ReturnsAsync(new OkResult());

        var controller = new GamesController(mockRepo.Object, null!);

        var result = await controller.PutGame(id, Game);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Tests the PutGame method of GamesController to ensure it returns a NotFoundResult when
    ///   trying to update a non-existing game.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutGame_ById_ReturnsNotFoundResult()
    {
        int id = 1;
        var Game = new Game { GameId = id, Title = "Dragon Ball Z Budokai Tenkaichi 3" };

        var mockRepo = new Mock<IGamesRepo>();
        mockRepo.Setup(r => r.PutGame(id, Game))
                .ReturnsAsync(new NotFoundResult());

        var controller = new GamesController(mockRepo.Object, null!);

        var result = await controller.PutGame(id, Game);

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the DeleteGame method of GamesController to ensure it returns an OkResult when an existing game is deleted successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteGame_ById_ReturnsOkResult()
    {
        int id = 1;

        var mockRepo = new Mock<IGamesRepo>();
        mockRepo.Setup(r => r.DeleteGame(id))
                .ReturnsAsync(new OkResult());

        var controller = new GamesController(mockRepo.Object, null!);

        var result = await controller.DeleteGame(id);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Tests the DeleteGame method of GamesController to ensure it returns a NotFoundResult when trying to delete a non-existing game.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteGame_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IGamesRepo>();
        mockRepo.Setup(r => r.DeleteGame(id))
                .ReturnsAsync(new NotFoundResult());

        var controller = new GamesController(mockRepo.Object, null!);

        var result = await controller.DeleteGame(id);

        Assert.IsType<NotFoundResult>(result);
    }
}

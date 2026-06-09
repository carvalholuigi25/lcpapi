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

public class AnimesControllerTests
{
    [Fact]
    public async Task GetAnimes_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var animes = new List<Anime>
        {
            new Anime { AnimeId = 1, Title = "Dragon Ball Super" },
            new Anime { AnimeId = 2, Title = "One Piece" },
            new Anime { AnimeId = 3, Title = "Naruto Shippuden" }
        };

        var mockRepo = new Mock<IAnimesRepo>();
        mockRepo.Setup(r => r.GetAnimes(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<Anime>>(animes));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(animes.Count);

        var controller = new AnimesController(mockRepo.Object);

        var result = await controller.GetAnimes(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<Anime>>(okResult.Value);

        Assert.Equal(animes.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Equal(3, response.Data.Count);
        Assert.Equal("Dragon Ball Super", response.Data[0].Title);
        Assert.Equal("One Piece", response.Data[1].Title);
        Assert.Equal("Naruto Shippuden", response.Data[2].Title);
    }

    [Fact]
    public async Task GetAnime_ById_ReturnsOkResult_WithQueryParamsResp()
    {
        int id = 1;
        var Anime = new Anime { AnimeId = id, Title = "Dragon Ball Super" };

        var mockRepo = new Mock<IAnimesRepo>();
        mockRepo.Setup(r => r.GetAnime(id))
                .ReturnsAsync(new ActionResult<Anime>(Anime));

        var controller = new AnimesController(mockRepo.Object);

        var result = await controller.GetAnime(id);

        var response = Assert.IsType<Anime>(result.Value);

        Assert.Equal(Anime.AnimeId, response.AnimeId);
        Assert.Equal(Anime.Title, response.Title);
    }

    [Fact]
    public async Task GetAnime_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IAnimesRepo>();
        mockRepo.Setup(r => r.GetAnime(id))
                .ReturnsAsync(new ActionResult<Anime>(new NotFoundResult()));

        var controller = new AnimesController(mockRepo.Object);

        var result = await controller.GetAnime(id);

        Assert.IsType<NotFoundResult>(result.Result);   
    }

    [Fact]
    public async Task CreateAnime_ReturnsCreatedAtActionResult()
    {
        var newAnime = new Anime { Title = "Dragon Ball Super" };
        var createdAnime = new Anime { AnimeId = 1, Title = "Dragon Ball Super" };

        var mockRepo = new Mock<IAnimesRepo>();
        mockRepo.Setup(r => r.CreateAnime(newAnime))
                .ReturnsAsync(new ActionResult<Anime>(new CreatedAtActionResult(nameof(AnimesController.GetAnime), "Animes", new { id = createdAnime.AnimeId }, createdAnime)));

        var controller = new AnimesController(mockRepo.Object);

        var result = await controller.CreateAnime(newAnime);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<Anime>(createdAtActionResult.Value);

        Assert.Equal(createdAnime.AnimeId, response.AnimeId);
        Assert.Equal(createdAnime.Title, response.Title);
    }

    [Fact]
    public async Task PutAnime_ById_ReturnsOkResult()
    {
        int id = 1;
        var Anime = new Anime { AnimeId = id, Title = "Dragon Ball Z" };

        var mockRepo = new Mock<IAnimesRepo>();
        mockRepo.Setup(r => r.PutAnime(id, Anime))
                .ReturnsAsync(new OkResult());

        var controller = new AnimesController(mockRepo.Object);

        var result = await controller.PutAnime(id, Anime);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task PutAnime_ById_ReturnsNotFoundResult()
    {
        int id = 1;
        var Anime = new Anime { AnimeId = id, Title = "Dragon Ball Z" };

        var mockRepo = new Mock<IAnimesRepo>();
        mockRepo.Setup(r => r.PutAnime(id, Anime))
                .ReturnsAsync(new NotFoundResult());

        var controller = new AnimesController(mockRepo.Object);

        var result = await controller.PutAnime(id, Anime);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAnime_ById_ReturnsOkResult()
    {
        int id = 1;

        var mockRepo = new Mock<IAnimesRepo>();
        mockRepo.Setup(r => r.DeleteAnime(id))
                .ReturnsAsync(new OkResult());

        var controller = new AnimesController(mockRepo.Object);

        var result = await controller.DeleteAnime(id);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteAnime_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IAnimesRepo>();
        mockRepo.Setup(r => r.DeleteAnime(id))
                .ReturnsAsync(new NotFoundResult());

        var controller = new AnimesController(mockRepo.Object);

        var result = await controller.DeleteAnime(id);

        Assert.IsType<NotFoundResult>(result);
    }
}

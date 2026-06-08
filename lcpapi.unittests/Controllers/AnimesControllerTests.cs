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
}

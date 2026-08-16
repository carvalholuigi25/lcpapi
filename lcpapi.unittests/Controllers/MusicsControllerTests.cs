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
/// Unit tests for MusicsController.
/// </summary>
public class MusicsControllerTests
{

    /// <summary>
    /// Tests the GetMusics method of MusicsController to ensure it returns an OkResult with a QueryParamsResp containing the expected data.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMusics_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var Musics = new List<Music>
        {
            new Music { MusicId = 1, Title = "Queens - Radio Ga Ga" },
        };

        var mockRepo = new Mock<IMusicsRepo>();
        mockRepo.Setup(r => r.GetMusics(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<Music>>(Musics));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(Musics.Count);

        var controller = new MusicsController(mockRepo.Object);

        var result = await controller.GetMusics(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<Music>>(okResult.Value);

        Assert.Equal(Musics.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal("Queens - Radio Ga Ga", response.Data[0].Title);
    }

    /// <summary>
    /// Tests the GetMusic method of MusicsController to ensure it returns an OkResult with the expected Musics data when a valid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMusic_ById_ReturnsOkResult_WithQueryParamsResp()
    {
        int id = 1;
        var Musics = new Music { MusicId = id, Title = "Queens - Radio Ga Ga" };

        var mockRepo = new Mock<IMusicsRepo>();
        mockRepo.Setup(r => r.GetMusic(id))
                .ReturnsAsync(new ActionResult<Music>(Musics));

        var controller = new MusicsController(mockRepo.Object);

        var result = await controller.GetMusic(id);

        var response = Assert.IsType<Music>(result.Value);

        Assert.Equal(Musics.MusicId, response.MusicId);
        Assert.Equal(Musics.Title, response.Title);
    }

    /// <summary>
    ///     Tests the GetMusic method of MusicsController to ensure it returns a NotFoundResult when an invalid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMusic_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IMusicsRepo>();
        mockRepo.Setup(r => r.GetMusic(id))
                .ReturnsAsync(new ActionResult<Music>(new NotFoundResult()));

        var controller = new MusicsController(mockRepo.Object);

        var result = await controller.GetMusic(id);

        Assert.IsType<NotFoundResult>(result.Result);   
    }

    /// <summary>
    /// Tests the CreateMusic method of MusicsController to ensure it returns a CreatedAtActionResult with the expected Musics data when a new Music is created successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateMusic_ReturnsCreatedAtActionResult()
    {
        var newMusic = new Music { Title = "Queens - Radio Ga Ga" };
        var createdMusic = new Music { MusicId = 1, Title = "Queens - Radio Ga Ga" };

        var mockRepo = new Mock<IMusicsRepo>();
        mockRepo.Setup(r => r.CreateMusic(newMusic))
                .ReturnsAsync(new ActionResult<Music>(new CreatedAtActionResult(nameof(MusicsController.GetMusic), "Musics", new { id = createdMusic.MusicId }, createdMusic)));

        var controller = new MusicsController(mockRepo.Object);

        var result = await controller.CreateMusic(newMusic);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<Music>(createdAtActionResult.Value);

        Assert.Equal(createdMusic.MusicId, response.MusicId);
        Assert.Equal(createdMusic.Title, response.Title);
    }


    /// <summary>
    /// Tests the PutMusic method of MusicsController to ensure it returns a NoContentResult when an existing Music is updated successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutMusic_ById_ReturnsOkResult()
    {
        int id = 1;
        var Music = new Music { MusicId = id, Title = "Queens - Radio Ga Ga" };

        var mockRepo = new Mock<IMusicsRepo>();
        mockRepo.Setup(r => r.PutMusic(id, Music))
                .ReturnsAsync(new OkResult());

        var controller = new MusicsController(mockRepo.Object);

        var result = await controller.PutMusic(id, Music);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///     Tests the PutMusic method of MusicsController to ensure it returns a NotFoundResult when trying to update a non-existing Music.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutMusic_ById_ReturnsNotFoundResult()
    {
        int id = 1;
        var Music = new Music { MusicId = id, Title = "Queens - Radio Ga Ga" };

        var mockRepo = new Mock<IMusicsRepo>();
        mockRepo.Setup(r => r.PutMusic(id, Music))
                .ReturnsAsync(new NotFoundResult());

        var controller = new MusicsController(mockRepo.Object);

        var result = await controller.PutMusic(id, Music);

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the DeleteMusic method of MusicsController to ensure it returns an OkResult when an existing Music is deleted successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteMusic_ById_ReturnsOkResult()
    {
        int id = 1;

        var mockRepo = new Mock<IMusicsRepo>();
        mockRepo.Setup(r => r.DeleteMusic(id))
                .ReturnsAsync(new OkResult());

        var controller = new MusicsController(mockRepo.Object);

        var result = await controller.DeleteMusic(id);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///    Tests the DeleteMusic method of MusicsController to ensure it returns a NotFoundResult when trying to delete a non-existing Music.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteMusic_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IMusicsRepo>();
        mockRepo.Setup(r => r.DeleteMusic(id))
                .ReturnsAsync(new NotFoundResult());

        var controller = new MusicsController(mockRepo.Object);

        var result = await controller.DeleteMusic(id);

        Assert.IsType<NotFoundResult>(result);
    }
}

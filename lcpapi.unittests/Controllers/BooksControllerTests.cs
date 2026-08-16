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
/// Unit tests for BooksController.
/// </summary>
public class BooksControllerTests
{

    /// <summary>
    /// Tests the GetBooks method of BooksController to ensure it returns an OkResult with a QueryParamsResp containing the expected data.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetBooks_ReturnsOkResult_WithQueryParamsResp()
    {
        var queryParams = new QueryParams();
        var Books = new List<Book>
        {
            new Book { BookId = 1, Title = "Harry Potter and the Philosopher's Stone" },
        };

        var mockRepo = new Mock<IBooksRepo>();
        mockRepo.Setup(r => r.GetBooks(It.Is<QueryParams>(q => q.Page == queryParams.Page && q.PageSize == queryParams.PageSize)))
                .ReturnsAsync(new ActionResult<IEnumerable<Book>>(Books));
        mockRepo.Setup(r => r.GetTotalCountAsync(It.IsAny<QueryParams>()))
                .ReturnsAsync(Books.Count);

        var controller = new BooksController(mockRepo.Object);

        var result = await controller.GetBooks(queryParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<QueryParamsResp<Book>>(okResult.Value);

        Assert.Equal(Books.Count, response.TotalCount);
        Assert.Equal(queryParams.Page, response.Page);
        Assert.Equal(queryParams.PageSize, response.PageSize);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal("Harry Potter and the Philosopher's Stone", response.Data[0].Title);
    }

    /// <summary>
    /// Tests the GetBook method of BooksController to ensure it returns an OkResult with the expected Book data when a valid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetBook_ById_ReturnsOkResult_WithQueryParamsResp()
    {
        int id = 1;
        var Book = new Book { BookId = id, Title = "Harry Potter and the Philosopher's Stone" };

        var mockRepo = new Mock<IBooksRepo>();
        mockRepo.Setup(r => r.GetBook(id))
                .ReturnsAsync(new ActionResult<Book>(Book));

        var controller = new BooksController(mockRepo.Object);

        var result = await controller.GetBook(id);

        var response = Assert.IsType<Book>(result.Value);

        Assert.Equal(Book.BookId, response.BookId);
        Assert.Equal(Book.Title, response.Title);
    }

    /// <summary>
    ///     Tests the GetBook method of BooksController to ensure it returns a NotFoundResult when an invalid ID is provided.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetBook_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IBooksRepo>();
        mockRepo.Setup(r => r.GetBook(id))
                .ReturnsAsync(new ActionResult<Book>(new NotFoundResult()));

        var controller = new BooksController(mockRepo.Object);

        var result = await controller.GetBook(id);

        Assert.IsType<NotFoundResult>(result.Result);   
    }

    /// <summary>
    /// Tests the CreateBook method of BooksController to ensure it returns a CreatedAtActionResult with the expected Book data when a new Book is created successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateBook_ReturnsCreatedAtActionResult()
    {
        var newBook = new Book { Title = "Avengers: Endgame" };
        var createdBook = new Book { BookId = 1, Title = "Avengers: Endgame" };

        var mockRepo = new Mock<IBooksRepo>();
        mockRepo.Setup(r => r.CreateBook(newBook))
                .ReturnsAsync(new ActionResult<Book>(new CreatedAtActionResult(nameof(BooksController.GetBook), "Books", new { id = createdBook.BookId }, createdBook)));

        var controller = new BooksController(mockRepo.Object);

        var result = await controller.CreateBook(newBook);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<Book>(createdAtActionResult.Value);

        Assert.Equal(createdBook.BookId, response.BookId);
        Assert.Equal(createdBook.Title, response.Title);
    }


    /// <summary>
    /// Tests the PutBook method of BooksController to ensure it returns a NoContentResult when an existing Book is updated successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutBook_ById_ReturnsOkResult()
    {
        int id = 1;
        var Book = new Book { BookId = id, Title = "Harry Potter and the Philosopher's Stone" };

        var mockRepo = new Mock<IBooksRepo>();
        mockRepo.Setup(r => r.PutBook(id, Book))
                .ReturnsAsync(new OkResult());

        var controller = new BooksController(mockRepo.Object);

        var result = await controller.PutBook(id, Book);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///     Tests the PutBook method of BooksController to ensure it returns a NotFoundResult when trying to update a non-existing Book.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task PutBook_ById_ReturnsNotFoundResult()
    {
        int id = 1;
        var Book = new Book { BookId = id, Title = "Harry Potter and the Philosopher's Stone" };

        var mockRepo = new Mock<IBooksRepo>();
        mockRepo.Setup(r => r.PutBook(id, Book))
                .ReturnsAsync(new NotFoundResult());

        var controller = new BooksController(mockRepo.Object);

        var result = await controller.PutBook(id, Book);

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the DeleteBook method of BooksController to ensure it returns an OkResult when an existing Book is deleted successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteBook_ById_ReturnsOkResult()
    {
        int id = 1;

        var mockRepo = new Mock<IBooksRepo>();
        mockRepo.Setup(r => r.DeleteBook(id))
                .ReturnsAsync(new OkResult());

        var controller = new BooksController(mockRepo.Object);

        var result = await controller.DeleteBook(id);

        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    ///    Tests the DeleteBook method of BooksController to ensure it returns a NotFoundResult when trying to delete a non-existing Book.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteBook_ById_ReturnsNotFoundResult()
    {
        int id = 1;

        var mockRepo = new Mock<IBooksRepo>();
        mockRepo.Setup(r => r.DeleteBook(id))
                .ReturnsAsync(new NotFoundResult());

        var controller = new BooksController(mockRepo.Object);

        var result = await controller.DeleteBook(id);

        Assert.IsType<NotFoundResult>(result);
    }
}

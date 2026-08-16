using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using lcpapi.Context;
using lcpapi.Hubs;
using lcpapi.Models;
using lcpapi.Models.QParams;
using lcpapi.Repositories;

namespace lcpapi.unittests.Repositories;

public class BooksRepoTests
{
    private readonly MyDBContext _context;
    private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
    private readonly BooksRepo _repo;

    public BooksRepoTests()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c.GetSection(It.IsAny<string>()).Value).Returns("MemoryDB");

        var options = new DbContextOptionsBuilder<MyDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MyDBContext(options, configMock.Object);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _hubContextMock = new Mock<IHubContext<ChatHub>>();
        var clientsMock = new Mock<IHubClients>();
        var clientProxyMock = new Mock<IClientProxy>();
        clientProxyMock
            .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        clientsMock.Setup(c => c.All).Returns(clientProxyMock.Object);
        _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

        _repo = new BooksRepo(_context, _hubContextMock.Object);
    }

    [Fact]
    public async Task GetBooks_ReturnsBooksFromContext()
    {
        // Arrange
        _context.Books.AddRange(
            new Book { Title = "Book A" },
            new Book { Title = "Book B" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        // Act
        var result = await _repo.GetBooks(queryParams);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetBook_WithExistingId_ReturnsBook()
    {
        // Arrange
        var book = new Book { Title = "My Book" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.GetBook(book.BookId);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(book.BookId, result.Value!.BookId);
        Assert.Equal("My Book", result.Value.Title);
    }

    [Fact]
    public async Task GetBook_WithMissingId_ReturnsNotFound()
    {
        // Act
        var result = await _repo.GetBook(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_AddsBookAndReturnsCreatedResult()
    {
        // Arrange
        var book = new Book { Title = "New Test Book" };

        // Act
        var result = await _repo.CreateBook(book);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedBook = Assert.IsType<Book>(createdResult.Value);
        Assert.Equal("New Test Book", returnedBook.Title);
        Assert.Equal(1, await _context.Books.CountAsync());
        Assert.Equal(book.BookId, returnedBook.BookId);
    }

    [Fact]
    public async Task PutBook_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var book = new Book { Title = "Existing" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var updatedBook = new Book { BookId = book.BookId + 1, Title = "Updated" };

        // Act
        var result = await _repo.PutBook(book.BookId, updatedBook);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteBook_RemovesBook_WhenExists()
    {
        // Arrange
        var book = new Book { Title = "Will Delete" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.DeleteBook(book.BookId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_context.Books);
    }

    [Fact]
    public async Task DeleteBook_WithMissingId_ReturnsNotFound()
    {
        // Act
        var result = await _repo.DeleteBook(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTotalCountAsync_ReturnsTotalCount()
    {
        // Arrange
        _context.Books.AddRange(
            new Book { Title = "A" },
            new Book { Title = "B" },
            new Book { Title = "C" }
        );
        await _context.SaveChangesAsync();

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        // Act
        var count = await _repo.GetTotalCountAsync(queryParams);

        // Assert
        Assert.Equal(3, count);
    }
}

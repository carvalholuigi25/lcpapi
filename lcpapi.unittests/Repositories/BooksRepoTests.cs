using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Repositories;
using lcpapi.Models.QParams;
using lcpapi.Hubs;

namespace lcpapi.unittests.Repositories;

public class BooksRepoTests
{
    private readonly Mock<MyDBContext> _mockContext;
    private readonly Mock<IHubContext<ChatHub>> _mockHubContext;
    private readonly BooksRepo _booksRepo;

    public BooksRepoTests()
    {
        _mockContext = new Mock<MyDBContext>();
        _mockHubContext = new Mock<IHubContext<ChatHub>>();
        _booksRepo = new BooksRepo(_mockContext.Object, _mockHubContext.Object);
    }

    #region GetBooks Tests

    [Fact]
    public async Task GetBooks_WithValidQueryParams_ReturnsOkResultWithBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book 1", Author = "Author 1" },
            new Book { BookId = 2, Title = "Book 2", Author = "Author 2" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(books.Provider);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(books.Expression);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(books.ElementType);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(books.GetEnumerator());
        mockDbSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);

        var queryParams = new QueryParams { Page = 1, PageSize = 10, SortBy = "id", SortOrder = SortOrderEnum.asc };

        // Act
        var result = await _booksRepo.GetBooks(queryParams);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
    }

    [Fact]
    public async Task GetBooks_WithSearchParam_FiltersResults()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { BookId = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald" },
            new Book { BookId = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(books.Provider);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(books.Expression);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(books.ElementType);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(books.GetEnumerator());
        mockDbSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);

        var queryParams = new QueryParams { Page = 1, PageSize = 10, SortBy = "title", SortOrder = SortOrderEnum.asc, Search = "Great" };

        // Act
        var result = await _booksRepo.GetBooks(queryParams);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region GetBook Tests

    [Fact]
    public async Task GetBook_WithValidId_ReturnsOkResultWithBook()
    {
        // Arrange
        var bookId = 1;
        var book = new Book { BookId = bookId, Title = "Test Book", Author = "Test Author" };

        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>(), CancellationToken.None))
            .ReturnsAsync(book);

        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);

        // Act
        var result = await _booksRepo.GetBook(bookId);

        // Assert
        Assert.NotNull(result);
        var okResult = result.Value;
        Assert.Equal(bookId, okResult?.BookId);
        Assert.Equal("Test Book", okResult?.Title);
    }

    [Fact]
    public async Task GetBook_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var bookId = 999;

        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Book, bool>>>(), CancellationToken.None))
            .ReturnsAsync((Book?)null);

        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);

        // Act
        var result = await _booksRepo.GetBook(bookId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Value);
    }

    #endregion

    #region CreateBook Tests

    [Fact]
    public async Task CreateBook_WithValidBook_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var newBook = new Book { BookId = 1, Title = "New Book", Author = "New Author", Description = "Test Description" };

        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbSet.Setup(m => m.Add(It.IsAny<Book>())).Verifiable();

        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        var result = await _booksRepo.CreateBook(newBook);

        // Assert
        Assert.NotNull(result);
        var createdResult = result.Value;
        Assert.Equal("New Book", createdResult?.Title);
        Assert.Equal("New Author", createdResult?.Author);
        _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateBook_CallsSaveChangesAsync()
    {
        // Arrange
        var newBook = new Book { BookId = 1, Title = "Another Book" };

        var mockDbSet = new Mock<DbSet<Book>>();
        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        await _booksRepo.CreateBook(newBook);

        // Assert
        _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    #endregion

    #region PutBook Tests

    [Fact]
    public async Task PutBook_WithMatchingIds_UpdatesBook()
    {
        // Arrange
        var bookId = 1;
        var book = new Book { BookId = bookId, Title = "Updated Book", Author = "Updated Author" };

        var mockDbSet = new Mock<DbSet<Book>>();
        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.Entry(It.IsAny<Book>())).Returns(new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Book>>().Object);
        _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        var result = await _booksRepo.PutBook(bookId, book);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
        _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task PutBook_WithMismatchedIds_ReturnsBadRequest()
    {
        // Arrange
        var bookId = 1;
        var book = new Book { BookId = 2, Title = "Test Book" };

        // Act
        var result = await _booksRepo.PutBook(bookId, book);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestResult>(result);
    }

    #endregion

    #region DeleteBook Tests

    [Fact]
    public async Task DeleteBook_WithValidId_DeletesBook()
    {
        // Arrange
        var bookId = 1;
        var book = new Book { BookId = bookId, Title = "Book to Delete" };

        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbSet.Setup(m => m.FindAsync(new object[] { bookId }, CancellationToken.None))
            .ReturnsAsync(book);

        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        var result = await _booksRepo.DeleteBook(bookId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
        mockDbSet.Verify(m => m.Remove(It.Is<Book>(b => b.BookId == bookId)), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteBook_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var bookId = 999;

        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbSet.Setup(m => m.FindAsync(new object[] { bookId }, CancellationToken.None))
            .ReturnsAsync((Book?)null);

        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);

        // Act
        var result = await _booksRepo.DeleteBook(bookId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region GetTotalCountAsync Tests

    [Fact]
    public async Task GetTotalCountAsync_WithNoFilter_ReturnsTotalCount()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book 1" },
            new Book { BookId = 2, Title = "Book 2" },
            new Book { BookId = 3, Title = "Book 3" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(books.Provider);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(books.Expression);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(books.ElementType);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(books.GetEnumerator());

        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);

        var queryParams = new QueryParams { Page = 1, PageSize = 10 };

        // Act
        var result = await _booksRepo.GetTotalCountAsync(queryParams);

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task GetTotalCountAsync_WithFilter_ReturnsFilteredCount()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { BookId = 1, Title = "The Great Gatsby" },
            new Book { BookId = 2, Title = "To Kill a Mockingbird" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(books.Provider);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(books.Expression);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(books.ElementType);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(books.GetEnumerator());

        _mockContext.Setup(c => c.Books).Returns(mockDbSet.Object);

        var queryParams = new QueryParams { Page = 1, PageSize = 10, SortBy = "title", Search = "Great" };

        // Act
        var result = await _booksRepo.GetTotalCountAsync(queryParams);

        // Assert
        Assert.True(result >= 0);
    }

    #endregion
}

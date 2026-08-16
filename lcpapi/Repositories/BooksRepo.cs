using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class BooksRepo : ControllerBase, IBooksRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public BooksRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<ActionResult<IEnumerable<Book>>> GetBooks(QueryParams queryParams)
    {
        var query = _context.Books.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        // Sorting
        query = GetSortByData(query, queryParams);

        // Pagination
        query = GetPaginationData(query, queryParams);

        var res = await query.Include(x => x.BooksMediasInfo).ToListAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", res);
        return res;
    }

    public async Task<ActionResult<Book>> GetBook(int? id)
    {
        // var Book = await _context.Books.FindAsync(id);

        var Book = await _context.Books
            .Include(g => g.BooksMediasInfo)
            .FirstOrDefaultAsync(m => m.BookId == id);

        if (Book == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Book);
        return Book;
    }

    public async Task<ActionResult<Book>> CreateBook(Book Book)
    {
        _context.Books.Add(Book);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBook), new { id = Book.BookId }, Book);
    }

    public async Task<IActionResult> PutBook(int? id, Book Book)
    {
        if (id != Book.BookId)
        {
            return BadRequest();
        }

        _context.Entry(Book).State = EntityState.Modified;

        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Book);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    public async Task<IActionResult> DeleteBook(int? id)
    {
        var Book = await _context.Books.FindAsync(id);
        if (Book == null)
        {
            return NotFound();
        }

        _context.Books.Remove(Book);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.Books.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool BookExists(int? id)
    {
        return _context.Books.Any(e => e.BookId == id);
    }

    private static IQueryable<Book> GetFilterData(IQueryable<Book> query, QueryParams queryParams)
    {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.Title!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.BookId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<Book> GetSortByData(IQueryable<Book> query, QueryParams queryParams)
    {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Title) : query.OrderBy(i => i.Title),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.BookId) : query.OrderBy(i => i.BookId),
            };
        }

        return query;
    }

    private static IQueryable<Book> GetPaginationData(IQueryable<Book> query, QueryParams queryParams)
    {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
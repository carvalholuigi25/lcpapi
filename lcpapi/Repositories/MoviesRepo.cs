using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class MoviesRepo : ControllerBase, IMoviesRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public MoviesRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    
    public async Task<ActionResult<IEnumerable<Movies>>> GetMovies(QueryParams queryParams)
    {
        var query =  _context.Movies.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        // Sorting
        query = GetSortByData(query, queryParams);

        // Pagination
        query = GetPaginationData(query, queryParams);

        var res = await query.Include(x => x.MoviesMediasInfo).ToListAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", res);
        return res;
    }

    public async Task<ActionResult<Movies>> GetMovie(int? id)
    {
        // var Movie = await _context.Movies.FindAsync(id);

        var Movie = await _context.Movies
            .Include(g => g.MoviesMediasInfo)
            .FirstOrDefaultAsync(m => m.MovieId == id);

        if (Movie == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Movie);
        return Movie;
    }

    public async Task<ActionResult<Movies>> CreateMovie(Movies Movie)
    {
        _context.Movies.Add(Movie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Movie);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMovie), new { id = Movie.MovieId }, Movie);
    }

    public async Task<IActionResult> PutMovie(int? id, Movies Movie)
    {
            if (id != Movie.MovieId)
            {
                return BadRequest();
            }

            _context.Entry(Movie).State = EntityState.Modified;

            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Movie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
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

    public async Task<IActionResult> DeleteMovie(int? id)
    {
        var Movie = await _context.Movies.FindAsync(id);
        if (Movie == null)
        {
            return NotFound();
        }

        _context.Movies.Remove(Movie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Movie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.Movies.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool MovieExists(int? id)
    {
        return _context.Movies.Any(e => e.MovieId == id);
    }

    private static IQueryable<Movies> GetFilterData(IQueryable<Movies> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.Title!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.MovieId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<Movies> GetSortByData(IQueryable<Movies> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Title) : query.OrderBy(i => i.Title),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.MovieId) : query.OrderBy(i => i.MovieId),
            };
        }

        return query;
    }

    private static IQueryable<Movies> GetPaginationData(IQueryable<Movies> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
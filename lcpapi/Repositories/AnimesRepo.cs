using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class AnimesRepo : ControllerBase, IAnimesRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public AnimesRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<ActionResult<IEnumerable<Anime>>> GetAnimes(QueryParams queryParams)
    {
        var query =  _context.Animes.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        // Sorting
        query = GetSortByData(query, queryParams);

        // Pagination
        query = GetPaginationData(query, queryParams);

        var res = await query.Include(x => x.AnimesMediasInfo).ToListAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", res);
        return res;
    }

    public async Task<ActionResult<Anime>> GetAnime(int? id)
    {
        // var Anime = await _context.Animes.FindAsync(id);

        var Anime = await _context.Animes
            .Include(g => g.AnimesMediasInfo)
            .FirstOrDefaultAsync(m => m.AnimeId == id);

        if (Anime == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Anime);
        return Anime;
    }

    public async Task<ActionResult<Anime>> CreateAnime(Anime Anime)
    {
        _context.Animes.Add(Anime);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Anime);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAnime), new { id = Anime.AnimeId }, Anime);
    }

    public async Task<IActionResult> PutAnime(int? id, Anime Anime)
    {
        if (id != Anime.AnimeId)
        {
            return BadRequest();
        }

        _context.Entry(Anime).State = EntityState.Modified;

        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Anime);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnimeExists(id))
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

    public async Task<IActionResult> DeleteAnime(int? id)
    {
        var Anime = await _context.Animes.FindAsync(id);
        if (Anime == null)
        {
            return NotFound();
        }

        _context.Animes.Remove(Anime);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Anime);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.Animes.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool AnimeExists(int? id)
    {
        return _context.Animes.Any(e => e.AnimeId == id);
    }

    private static IQueryable<Anime> GetFilterData(IQueryable<Anime> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.Title!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.AnimeId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<Anime> GetSortByData(IQueryable<Anime> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Title) : query.OrderBy(i => i.Title),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.AnimeId) : query.OrderBy(i => i.AnimeId),
            };
        }

        return query;
    }

    private static IQueryable<Anime> GetPaginationData(IQueryable<Anime> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
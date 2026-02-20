using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class TvseriesSeasonsRepo : ControllerBase, ITvseriesSeasonsRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public TvseriesSeasonsRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    
    public async Task<ActionResult<IEnumerable<TvseriesSeasonsInfo>>> GetTvseriesSeasons(QueryParams queryParams)
    {
        var query =  _context.TvseriesSeasonsInfo.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        // Sorting
        query = GetSortByData(query, queryParams);

        // Pagination
        query = GetPaginationData(query, queryParams);

        var res = await query.ToListAsync();
        
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", res);
        return res;
    }

    public async Task<ActionResult<TvseriesSeasonsInfo>> GetTvserieSeasons(int? id)
    {
        // var Tvserie = await _context.TvseriesSeasons.FindAsync(id);

        var Tvserie = await _context.TvseriesSeasonsInfo.FirstOrDefaultAsync(m => m.SeasonsId == id);

        if (Tvserie == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        return Tvserie;
    }

    public async Task<ActionResult<TvseriesSeasonsInfo>> CreateTvserieSeasons(TvseriesSeasonsInfo Tvserie)
    {
        _context.TvseriesSeasonsInfo.Add(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTvserieSeasons), new { id = Tvserie.SeasonsId }, Tvserie);
    }

    public async Task<IActionResult> PutTvserieSeasons(int? id, TvseriesSeasonsInfo Tvserie)
    {
            if (id != Tvserie.SeasonsId)
            {
                return BadRequest();
            }

            _context.Entry(Tvserie).State = EntityState.Modified;

            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TvseriesSeasonsInfoExists(id))
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

    public async Task<IActionResult> DeleteTvserieSeasons(int? id)
    {
        var Tvserie = await _context.TvseriesSeasonsInfo.FindAsync(id);
        if (Tvserie == null)
        {
            return NotFound();
        }

        _context.TvseriesSeasonsInfo.Remove(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

     public async Task<IActionResult> DeleteAllTvserieSeasons()
    {
        var Tvserie = await _context.TvseriesSeasonsInfo.ToListAsync();
        if (Tvserie.Count == 0)
        {
            return NotFound();
        }

        _context.TvseriesSeasonsInfo.RemoveRange(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.TvseriesSeasonsInfo.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool TvseriesSeasonsInfoExists(int? id)
    {
        return _context.TvseriesSeasonsInfo.Any(e => e.SeasonsId == id);
    }

    private static IQueryable<TvseriesSeasonsInfo> GetFilterData(IQueryable<TvseriesSeasonsInfo> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.SeasonsTitle!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.SeasonsId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<TvseriesSeasonsInfo> GetSortByData(IQueryable<TvseriesSeasonsInfo> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.SeasonsTitle) : query.OrderBy(i => i.SeasonsTitle),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.SeasonsTitle) : query.OrderBy(i => i.SeasonsTitle),
            };
        }

        return query;
    }

    private static IQueryable<TvseriesSeasonsInfo> GetPaginationData(IQueryable<TvseriesSeasonsInfo> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
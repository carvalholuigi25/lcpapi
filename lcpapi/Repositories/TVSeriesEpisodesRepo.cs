using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class TvseriesEpisodesRepo : ControllerBase, ITvseriesEpisodesRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public TvseriesEpisodesRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    
    public async Task<ActionResult<IEnumerable<TvseriesEpisodesInfo>>> GetTvseriesEpisodes(QueryParams queryParams)
    {
        var query =  _context.TvseriesEpisodesInfos.AsQueryable();

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

    public async Task<ActionResult<TvseriesEpisodesInfo>> GetTvserieEpisodes(int? id)
    {
        // var Tvserie = await _context.TvseriesEpisodes.FindAsync(id);

        var Tvserie = await _context.TvseriesEpisodesInfos.FirstOrDefaultAsync(m => m.EpisodesId == id);

        if (Tvserie == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        return Tvserie;
    }

    public async Task<ActionResult<TvseriesEpisodesInfo>> CreateTvserieEpisodes(TvseriesEpisodesInfo Tvserie)
    {
        _context.TvseriesEpisodesInfos.Add(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTvserieEpisodes), new { id = Tvserie.EpisodesId }, Tvserie);
    }

    public async Task<IActionResult> PutTvserieEpisodes(int? id, TvseriesEpisodesInfo Tvserie)
    {
            if (id != Tvserie.EpisodesId)
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
                if (!TvseriesEpisodesInfosExists(id))
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

    public async Task<IActionResult> DeleteTvserieEpisodes(int? id)
    {
        var Tvserie = await _context.TvseriesEpisodesInfos.FindAsync(id);
        if (Tvserie == null)
        {
            return NotFound();
        }

        _context.TvseriesEpisodesInfos.Remove(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

     public async Task<IActionResult> DeleteAllTvserieEpisodes()
    {
        var Tvserie = await _context.TvseriesEpisodesInfos.ToListAsync();
        if (Tvserie.Count == 0)
        {
            return NotFound();
        }

        _context.TvseriesEpisodesInfos.RemoveRange(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.TvseriesEpisodesInfos.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool TvseriesEpisodesInfosExists(int? id)
    {
        return _context.TvseriesEpisodesInfos.Any(e => e.EpisodesId == id);
    }

    private static IQueryable<TvseriesEpisodesInfo> GetFilterData(IQueryable<TvseriesEpisodesInfo> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.EpisodesTitle!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.EpisodesId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<TvseriesEpisodesInfo> GetSortByData(IQueryable<TvseriesEpisodesInfo> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.EpisodesTitle) : query.OrderBy(i => i.EpisodesTitle),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.EpisodesTitle) : query.OrderBy(i => i.EpisodesTitle),
            };
        }

        return query;
    }

    private static IQueryable<TvseriesEpisodesInfo> GetPaginationData(IQueryable<TvseriesEpisodesInfo> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
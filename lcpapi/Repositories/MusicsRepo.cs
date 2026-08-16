using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class MusicsRepo : ControllerBase, IMusicsRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public MusicsRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<ActionResult<IEnumerable<Music>>> GetMusics(QueryParams queryParams)
    {
        var query =  _context.Musics.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        // Sorting
        query = GetSortByData(query, queryParams);

        // Pagination
        query = GetPaginationData(query, queryParams);

        var res = await query.Include(x => x.MusicsMediasInfo).ToListAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", res);
        return res;
    }

    public async Task<ActionResult<Music>> GetMusic(int? id)
    {
        // var Music = await _context.Musics.FindAsync(id);

        var Music = await _context.Musics
            .Include(g => g.MusicsMediasInfo)
            .FirstOrDefaultAsync(m => m.MusicId == id);

        if (Music == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Music);
        return Music;
    }

    public async Task<ActionResult<Music>> CreateMusic(Music Music)
    {
        _context.Musics.Add(Music);
        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Music);
        return CreatedAtAction(nameof(GetMusic), new { id = Music.MusicId }, Music);
    }

    public async Task<IActionResult> PutMusic(int? id, Music Music)
    {
            if (id != Music.MusicId)
            {
                return BadRequest();
            }

            _context.Entry(Music).State = EntityState.Modified;

            try
            {        
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Music);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MusicExists(id))
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

    public async Task<IActionResult> DeleteMusic(int? id)
    {
        var Music = await _context.Musics.FindAsync(id);
        if (Music == null)
        {
            return NotFound();
        }

        _context.Musics.Remove(Music);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Music);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.Musics.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool MusicExists(int? id)
    {
        return _context.Musics.Any(e => e.MusicId == id);
    }

    private static IQueryable<Music> GetFilterData(IQueryable<Music> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.Title!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.MusicId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<Music> GetSortByData(IQueryable<Music> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Title) : query.OrderBy(i => i.Title),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.MusicId) : query.OrderBy(i => i.MusicId),
            };
        }

        return query;
    }

    private static IQueryable<Music> GetPaginationData(IQueryable<Music> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
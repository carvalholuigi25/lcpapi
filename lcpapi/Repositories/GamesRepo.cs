using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class GamesRepo : ControllerBase, IGamesRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public GamesRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<ActionResult<IEnumerable<Game>>> GetGames(QueryParams queryParams)
    {
        var query =  _context.Games.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        // Sorting
        query = GetSortByData(query, queryParams);

        // Pagination
        query = GetPaginationData(query, queryParams);

        var result = await query.Include(x => x.GamesMediasInfo).ToListAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", result);
        return result;
    }

    public async Task<ActionResult<Game>> GetGame(int? id)
    {
        // var Game = await _context.Games.FindAsync(id);

        var Game = await _context.Games
            .Include(g => g.GamesMediasInfo)
            .FirstOrDefaultAsync(m => m.GameId == id);

        if (Game == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Game);
        return Game;
    }

    public async Task<ActionResult<Game>> CreateGame(Game Game)
    {
        _context.Games.Add(Game);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Game);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGame), new { id = Game.GameId }, Game);
    }

    public async Task<IActionResult> PutGame(int? id, Game Game)
    {
        if (id != Game.GameId)
        {
            return BadRequest();
        }

        _context.Entry(Game).State = EntityState.Modified;

        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Game);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GameExists(id))
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

    public async Task<IActionResult> DeleteGame(int? id)
    {
        var Game = await _context.Games.FindAsync(id);
        if (Game == null)
        {
            return NotFound();
        }

        _context.Games.Remove(Game);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Game);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.Games.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool GameExists(int? id)
    {
        return _context.Games.Any(e => e.GameId == id);
    }

    private static IQueryable<Game> GetFilterData(IQueryable<Game> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.Title!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.GameId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<Game> GetSortByData(IQueryable<Game> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Title) : query.OrderBy(i => i.Title),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.GameId) : query.OrderBy(i => i.GameId),
            };
        }

        return query;
    }

    private static IQueryable<Game> GetPaginationData(IQueryable<Game> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class ActionFiguresRepo : ControllerBase, IActionFiguresRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public ActionFiguresRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<ActionResult<IEnumerable<ActionFigure>>> GetActionFigures(QueryParams queryParams)
    {
        var query =  _context.ActionFigures.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        // Sorting
        query = GetSortByData(query, queryParams);

        // Pagination
        query = GetPaginationData(query, queryParams);

        var res = await query.Include(x => x.ActionFiguresMediasInfo).ToListAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", res);
        return res;
    }

    public async Task<ActionResult<ActionFigure>> GetActionFigure(int? id)
    {
        // var ActionFigure = await _context.ActionFigures.FindAsync(id);

        var ActionFigure = await _context.ActionFigures
            .Include(g => g.ActionFiguresMediasInfo)
            .FirstOrDefaultAsync(m => m.ActionFigureId == id);

        if (ActionFigure == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", ActionFigure);

        return ActionFigure;
    }

    public async Task<ActionResult<ActionFigure>> CreateActionFigure(ActionFigure ActionFigure)
    {
            _context.ActionFigures.Add(ActionFigure);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", ActionFigure);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetActionFigure), new { id = ActionFigure.ActionFigureId }, ActionFigure);
    }

    public async Task<IActionResult> PutActionFigure(int? id, ActionFigure ActionFigure)
    {
        if (id != ActionFigure.ActionFigureId)
            {
                return BadRequest();
            }

            _context.Entry(ActionFigure).State = EntityState.Modified;

            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", ActionFigure);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActionFigureExists(id))
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

    public async Task<IActionResult> DeleteActionFigure(int? id)
    {
        var ActionFigure = await _context.ActionFigures.FindAsync(id);
        if (ActionFigure == null)
        {
            return NotFound();
        }

        _context.ActionFigures.Remove(ActionFigure);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", ActionFigure);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.ActionFigures.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool ActionFigureExists(int? id)
    {
        return _context.ActionFigures.Any(e => e.ActionFigureId == id);
    }

    private static IQueryable<ActionFigure> GetFilterData(IQueryable<ActionFigure> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "name" => query.Where(i => i.Name.Contains(queryParams.Search)),
                    _ => query.Where(i => i.ActionFigureId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<ActionFigure> GetSortByData(IQueryable<ActionFigure> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "name" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.ActionFigureId) : query.OrderBy(i => i.ActionFigureId),
            };
        }

        return query;
    }

    private static IQueryable<ActionFigure> GetPaginationData(IQueryable<ActionFigure> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
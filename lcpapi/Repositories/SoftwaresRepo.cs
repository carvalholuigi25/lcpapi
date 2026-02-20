using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class SoftwaresRepo : ControllerBase, ISoftwaresRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public SoftwaresRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<ActionResult<IEnumerable<Software>>> GetSoftwares(QueryParams queryParams)
    {
        var query =  _context.Softwares.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        // Sorting
        query = GetSortByData(query, queryParams);

        // Pagination
        query = GetPaginationData(query, queryParams);

        var res = await query.Include(x => x.SoftwaresMediaInfos).ToListAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", res);
        return res;
    }

    public async Task<ActionResult<Software>> GetSoftware(int? id)
    {
        // var Software = await _context.Softwares.FindAsync(id);

        var Software = await _context.Softwares
            .Include(g => g.SoftwaresMediaInfos)
            .FirstOrDefaultAsync(m => m.SoftwareId == id);

        if (Software == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Software);
        return Software;
    }

    public async Task<ActionResult<Software>> CreateSoftware(Software Software)
    {
        _context.Softwares.Add(Software);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Software);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSoftware), new { id = Software.SoftwareId }, Software);
    }

    public async Task<IActionResult> PutSoftware(int? id, Software Software)
    {
        if (id != Software.SoftwareId)
        {
            return BadRequest();
        }

        _context.Entry(Software).State = EntityState.Modified;

        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Software);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SoftwareExists(id))
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

    public async Task<IActionResult> DeleteSoftware(int? id)
    {
        var Software = await _context.Softwares.FindAsync(id);
        if (Software == null)
        {
            return NotFound();
        }

        _context.Softwares.Remove(Software);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Software);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.Softwares.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool SoftwareExists(int? id)
    {
        return _context.Softwares.Any(e => e.SoftwareId == id);
    }

    private static IQueryable<Software> GetFilterData(IQueryable<Software> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.Name!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.SoftwareId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<Software> GetSortByData(IQueryable<Software> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.SoftwareId) : query.OrderBy(i => i.SoftwareId),
            };
        }

        return query;
    }

    private static IQueryable<Software> GetPaginationData(IQueryable<Software> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class PetRepo : ControllerBase, IPetsRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public PetRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    
    public async Task<ActionResult<IEnumerable<Pet>>> GetPets(QueryParams queryParams)
    {
        var query =  _context.Pets.AsQueryable();

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

    public async Task<ActionResult<Pet>> GetPet(int? id)
    {
        // var Pet = await _context.Pet.FindAsync(id);

        var Pet = await _context.Pets.FirstOrDefaultAsync(m => m.PetsId == id);

        if (Pet == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Pet);
        return Pet;
    }

    public async Task<ActionResult<Pet>> CreatePet(Pet Pet)
    {
        _context.Pets.Add(Pet);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Pet);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPet), new { id = Pet.PetsId }, Pet);
    }

    public async Task<IActionResult> PutPet(int? id, Pet Pet)
    {
            if (id != Pet.PetsId)
            {
                return BadRequest();
            }

            _context.Entry(Pet).State = EntityState.Modified;

            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Pet);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(id))
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

    public async Task<IActionResult> DeletePet(int? id)
    {
        var Pet = await _context.Pets.FindAsync(id);
        if (Pet == null)
        {
            return NotFound();
        }

        _context.Pets.Remove(Pet);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Pet);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.Pets.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool PetExists(int? id)
    {
        return _context.Pets.Any(e => e.PetsId == id);
    }

    private static IQueryable<Pet> GetFilterData(IQueryable<Pet> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "name" => query.Where(i => i.Name!.Contains(queryParams.Search)),
                    "type" => query.Where(i => i.Type!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.PetsId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<Pet> GetSortByData(IQueryable<Pet> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "name" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name),
                "type" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Type) : query.OrderBy(i => i.Type),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.PetsId) : query.OrderBy(i => i.PetsId),
            };
        }

        return query;
    }

    private static IQueryable<Pet> GetPaginationData(IQueryable<Pet> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
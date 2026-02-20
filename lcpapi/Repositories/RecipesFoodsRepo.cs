using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class RecipesFoodsRepo : ControllerBase, IRecipesFoodsRepo
{
     private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public RecipesFoodsRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }


    public async Task<ActionResult<IEnumerable<RecipesFoods>>> GetRecipesFoods(QueryParams queryParams)
    {
        var query =  _context.RecipesFoods.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        // Sorting
        query = GetSortByData(query, queryParams);

        // Pagination
        query = GetPaginationData(query, queryParams);

        var res = await query.Include(x => x.RecipesFoodsMediasInfo).ToListAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", res);
        return res;
    }

    public async Task<ActionResult<RecipesFoods>> GetRecipesFood(int? id)
    {
        // var RecipesFood = await _context.RecipesFoods.FindAsync(id);

        var RecipesFood = await _context.RecipesFoods
            .Include(g => g.RecipesFoodsMediasInfo)
            .FirstOrDefaultAsync(m => m.RecipesFoodsId == id);

        if (RecipesFood == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", RecipesFood);
        return RecipesFood;
    }

    public async Task<ActionResult<RecipesFoods>> CreateRecipesFoods(RecipesFoods RecipesFood)
    {
        _context.RecipesFoods.Add(RecipesFood);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", RecipesFood);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRecipesFood), new { id = RecipesFood.RecipesFoodsId }, RecipesFood);
    }

    public async Task<IActionResult> PutRecipesFoods(int? id, RecipesFoods RecipesFood)
    {
            if (id != RecipesFood.RecipesFoodsId)
            {
                return BadRequest();
            }

            _context.Entry(RecipesFood).State = EntityState.Modified;

            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", RecipesFood);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipesFoodExists(id))
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

    public async Task<IActionResult> DeleteRecipesFoods(int? id)
    {
            var RecipesFood = await _context.RecipesFoods.FindAsync(id);
            if (RecipesFood == null)
            {
                return NotFound();
            }

            _context.RecipesFoods.Remove(RecipesFood);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", RecipesFood);
            await _context.SaveChangesAsync();

            return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.RecipesFoods.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool RecipesFoodExists(int? id)
    {
        return _context.RecipesFoods.Any(e => e.RecipesFoodsId == id);
    }

    private static IQueryable<RecipesFoods> GetFilterData(IQueryable<RecipesFoods> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.Title!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.RecipesFoodsId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<RecipesFoods> GetSortByData(IQueryable<RecipesFoods> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Title) : query.OrderBy(i => i.Title),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.RecipesFoodsId) : query.OrderBy(i => i.RecipesFoodsId),
            };
        }

        return query;
    }

    private static IQueryable<RecipesFoods> GetPaginationData(IQueryable<RecipesFoods> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
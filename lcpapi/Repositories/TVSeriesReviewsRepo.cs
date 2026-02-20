using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class TvseriesReviewsRepo : ControllerBase, ITvseriesReviewsRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public TvseriesReviewsRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    
    public async Task<ActionResult<IEnumerable<TvseriesReviewsInfo>>> GetTvseriesReviews(QueryParams queryParams)
    {
        var query =  _context.TvseriesReviewsInfos.AsQueryable();

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

    public async Task<ActionResult<TvseriesReviewsInfo>> GetTvserieReviews(int? id)
    {
        // var Tvserie = await _context.TvseriesReviews.FindAsync(id);

        var Tvserie = await _context.TvseriesReviewsInfos.FirstOrDefaultAsync(m => m.ReviewsId == id);

        if (Tvserie == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        return Tvserie;
    }

    public async Task<ActionResult<TvseriesReviewsInfo>> CreateTvserieReviews(TvseriesReviewsInfo Tvserie)
    {
        _context.TvseriesReviewsInfos.Add(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTvserieReviews), new { id = Tvserie.ReviewsId }, Tvserie);
    }

    public async Task<IActionResult> PutTvserieReviews(int? id, TvseriesReviewsInfo Tvserie)
    {
            if (id != Tvserie.ReviewsId)
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
                if (!TvseriesReviewsInfoExists(id))
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

    public async Task<IActionResult> DeleteTvserieReviews(int? id)
    {
        var Tvserie = await _context.TvseriesReviewsInfos.FindAsync(id);
        if (Tvserie == null)
        {
            return NotFound();
        }

        _context.TvseriesReviewsInfos.Remove(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

     public async Task<IActionResult> DeleteAllTvserieReviews()
    {
        var Tvserie = await _context.TvseriesReviewsInfos.ToListAsync();
        if (Tvserie.Count == 0)
        {
            return NotFound();
        }

        _context.TvseriesReviewsInfos.RemoveRange(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.TvseriesReviewsInfos.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool TvseriesReviewsInfoExists(int? id)
    {
        return _context.TvseriesReviewsInfos.Any(e => e.ReviewsId == id);
    }

    private static IQueryable<TvseriesReviewsInfo> GetFilterData(IQueryable<TvseriesReviewsInfo> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.ReviewsTitle!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.ReviewsId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<TvseriesReviewsInfo> GetSortByData(IQueryable<TvseriesReviewsInfo> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.ReviewsTitle) : query.OrderBy(i => i.ReviewsTitle),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.ReviewsTitle) : query.OrderBy(i => i.ReviewsTitle),
            };
        }

        return query;
    }

    private static IQueryable<TvseriesReviewsInfo> GetPaginationData(IQueryable<TvseriesReviewsInfo> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
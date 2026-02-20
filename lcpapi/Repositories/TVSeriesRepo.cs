using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;
using NuGet.Protocol;

namespace lcpapi.Repositories;

public class TvseriesRepo : ControllerBase, ITvseriesRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public TvseriesRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    
    public async Task<ActionResult<IEnumerable<Tvseries>>> GetTvseries(QueryParams queryParams)
    {
        var query =  _context.Tvseries.AsQueryable();

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

     public async Task<IActionResult> GetTvseriesAllInfo()
    {
        var tvseriesList = await _context.Tvseries.ToListAsync();
        var seasonsList = await _context.TvseriesSeasonsInfo.ToListAsync();
        var episodesList = await _context.TvseriesEpisodesInfos.ToListAsync();
        var reviewsList = await _context.TvseriesReviewsInfos.ToListAsync();

        var result = tvseriesList.Select(tvs => new
        {
            TvserieId = tvs.TvserieId,
            Title = tvs.Title,
            Description = tvs.Description,
            Image = tvs.Image,
            Artwork = tvs.Artwork,
            Studio = tvs.Studio,
            IsFeatured = tvs.IsFeatured,
            ScoreRating = tvs.ScoreRating,
            NumSeasons = tvs.NumSeasons,
            NumEpisodes = tvs.NumEpisodes,
            ReleaseDate = tvs.ReleaseDate,
            Genre = tvs.Genre,
            Format = tvs.Format,
            SeasonsInfo = seasonsList.Where(s => s.TvserieId == tvs.TvserieId).ToList(),
            EpisodesInfos = episodesList.Where(e => e.TvserieId == tvs.TvserieId).ToList(),
            ReviewsInfos = reviewsList.Where(r => r.TvserieId == tvs.TvserieId).ToList(),
            TvseriesMediasInfo = tvs.MediasInfo
        }).ToList();

        return Ok(result);
    }

    public async Task<IActionResult> GetTvseriesAllInfoById(int? id)
    {
        if (id == null || id <= 0)
        {
            return BadRequest("Invalid TV series ID");
        }

        var tvSeries = await _context.Tvseries.FirstOrDefaultAsync(tvs => tvs.TvserieId == id);
        if (tvSeries == null)
        {
            return NotFound();
        }

        var seasonsList = await _context.TvseriesSeasonsInfo.Where(s => s.TvserieId == id).ToListAsync();
        var episodesList = await _context.TvseriesEpisodesInfos.Where(e => e.TvserieId == id).ToListAsync();
        var reviewsList = await _context.TvseriesReviewsInfos.Where(e => e.TvserieId == id).ToListAsync();

        var result = new
        {
            TvserieId = tvSeries.TvserieId,
            Title = tvSeries.Title,
            Description = tvSeries.Description,
            Image = tvSeries.Image,
            Artwork = tvSeries.Artwork,
            Studio = tvSeries.Studio,
            IsFeatured = tvSeries.IsFeatured,
            ScoreRating = tvSeries.ScoreRating,
            NumSeasons = tvSeries.NumSeasons,
            NumEpisodes = tvSeries.NumEpisodes,
            ReleaseDate = tvSeries.ReleaseDate,
            Genre = tvSeries.Genre,
            Format = tvSeries.Format,
            SeasonsInfo = seasonsList,
            EpisodesInfos = episodesList,
            ReviewsInfos = reviewsList,
            TvseriesMediasInfo = tvSeries.MediasInfo
        };

        return Ok(result);
    }

    public async Task<ActionResult<Tvseries>> GetTvserie(int? id)
    {
        // var Tvserie = await _context.Tvseries.FindAsync(id);

        var Tvserie = await _context.Tvseries.FirstOrDefaultAsync(m => m.TvserieId == id);

        if (Tvserie == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        return Tvserie;
    }

    public async Task<ActionResult<Tvseries>> CreateTvserie(Tvseries Tvserie)
    {
        _context.Tvseries.Add(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTvserie), new { id = Tvserie.TvserieId }, Tvserie);
    }

    public async Task<IActionResult> PutTvserie(int? id, Tvseries Tvserie)
    {
            if (id != Tvserie.TvserieId)
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
                if (!TvserieExists(id))
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

    public async Task<IActionResult> DeleteTvserie(int? id)
    {
        var Tvserie = await _context.Tvseries.FindAsync(id);
        if (Tvserie == null)
        {
            return NotFound();
        }

        _context.Tvseries.Remove(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

     public async Task<IActionResult> DeleteAllTvserie()
    {
        var Tvserie = await _context.Tvseries.ToListAsync();
        if (Tvserie.Count == 0)
        {
            return NotFound();
        }

        _context.Tvseries.RemoveRange(Tvserie);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", Tvserie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.Tvseries.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool TvserieExists(int? id)
    {
        return _context.Tvseries.Any(e => e.TvserieId == id);
    }

    private static IQueryable<Tvseries> GetFilterData(IQueryable<Tvseries> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => query.Where(i => i.Title!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.TvserieId == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<Tvseries> GetSortByData(IQueryable<Tvseries> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "title" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Title) : query.OrderBy(i => i.Title),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.TvserieId) : query.OrderBy(i => i.TvserieId),
            };
        }

        return query;
    }

    private static IQueryable<Tvseries> GetPaginationData(IQueryable<Tvseries> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
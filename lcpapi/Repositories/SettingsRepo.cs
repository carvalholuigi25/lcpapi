using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class SettingsRepo : ControllerBase, ISettingsRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public SettingsRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<ActionResult<IEnumerable<Settings>>> GetSettings(QueryParams queryParams)
    {
        var query =  _context.Settings.AsQueryable();

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

    public async Task<ActionResult<Settings>> GetSetting(int? id)
    {
        var setting = await _context.Settings.FindAsync(id);

        if (setting == null)
        {
            return NotFound();
        }

        return setting;
    }

    public async Task<ActionResult<Settings>> PostSetting(Settings setting)
    {
        if(!string.IsNullOrEmpty(setting.ThemeSettingName) &&  _context.Settings.Where(x => x.ThemeSettingName == setting.ThemeSettingName).Count() == 1) {
            return BadRequest("Settingname already exists!");
        }

        _context.Settings.Add(setting);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", setting);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSetting), new { id = setting.Id }, setting);
    }

    public async Task<IActionResult> PutSetting(int? id, Settings setting)
    {
        if (id != setting.Id)
        {
            return BadRequest();
        }

        if(!string.IsNullOrEmpty(setting.ThemeSettingName) &&  _context.Settings.Where(x => x.ThemeSettingName == setting.ThemeSettingName).Count() == 1) {
            return BadRequest("ThemeSettingName already exists!");
        }

        _context.Entry(setting).State = EntityState.Modified;

        try
        {        
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", setting);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SettingExists(id))
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

    public async Task<IActionResult> DeleteSetting(int? id)
    {
        var setting = await _context.Settings.FindAsync(id);
        if (setting == null)
        {
            return NotFound();
        }

        _context.Settings.Remove(setting);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", setting);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.Settings.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool SettingExists(int? id)
    {
        return _context.Settings.Any(e => e.Id == id);
    }

    private static IQueryable<Settings> GetFilterData(IQueryable<Settings> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                var sortorderval = queryParams.SortOrder!.Value.ToString();
                StringComparison strcom = StringComparison.OrdinalIgnoreCase;
                query = queryParams.SortBy.ToLower() switch
                {
                    "ThemeSettingName" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.ThemeSettingName) : query.OrderBy(i => i.ThemeSettingName),
                    _ => query.Where(i => i.Id == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<Settings> GetSortByData(IQueryable<Settings> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "ThemeSettingName" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.ThemeSettingName) : query.OrderBy(i => i.ThemeSettingName),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Id) : query.OrderBy(i => i.Id),
            };
        }

        return query;
    }

    private static IQueryable<Settings> GetPaginationData(IQueryable<Settings> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
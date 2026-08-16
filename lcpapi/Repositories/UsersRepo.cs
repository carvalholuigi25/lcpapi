using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class UsersRepo : ControllerBase, IUsersRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public UsersRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<ActionResult<IEnumerable<User>>> GetUsers(QueryParams queryParams)
    {
        var query =  _context.Users.AsQueryable();

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

    public async Task<ActionResult<User>> GetUser(int? id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    public async Task<ActionResult<User>> PostUser(User user)
    {
        if(!string.IsNullOrEmpty(user.Password)) {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, 10, false);
        }

        if(!string.IsNullOrEmpty(user.Username) &&  _context.Users.Where(x => x.Username == user.Username).Count() == 1) {
            return BadRequest("Username already exists!");
        }

        _context.Users.Add(user);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    public async Task<IActionResult> PutUser(int? id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        if(!string.IsNullOrEmpty(user.Password)) {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, 10, false);
        }

        if(!string.IsNullOrEmpty(user.Username) &&  _context.Users.Where(x => x.Username == user.Username).Count() == 1) {
            return BadRequest("Username already exists!");
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {        
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", user);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
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

    public async Task<IActionResult> DeleteUser(int? id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public async Task<int> GetTotalCountAsync(QueryParams queryParams)
    {
        var query = _context.Users.AsQueryable();

        // Filtering
        query = GetFilterData(query, queryParams);

        return await query.CountAsync();
    }

    private bool UserExists(int? id)
    {
        return _context.Users.Any(e => e.Id == id);
    }

    private static IQueryable<User> GetFilterData(IQueryable<User> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "username" => query.Where(i => i.Username.Contains(queryParams.Search)),
                    "displayname" => query.Where(i => i.DisplayName!.Contains(queryParams.Search)),
                    "role" => query.Where(i => i.Role.ToString()!.Contains(queryParams.Search)),
                    _ => query.Where(i => i.Id == int.Parse(queryParams.Search)),
                };
            }
        }

        return query;
    }

    private static IQueryable<User> GetSortByData(IQueryable<User> query, QueryParams queryParams) {
        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortorderval = queryParams.SortOrder!.Value.ToString();
            StringComparison strcom = StringComparison.OrdinalIgnoreCase;
            query = queryParams.SortBy.ToLower() switch
            {
                "username" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Username) : query.OrderBy(i => i.Username),
                "displayname" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.DisplayName) : query.OrderBy(i => i.DisplayName),
                "role" => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Role) : query.OrderBy(i => i.Role),
                _ => sortorderval.Contains("desc", strcom) ? query.OrderByDescending(i => i.Id) : query.OrderBy(i => i.Id),
            };
        }

        return query;
    }

    private static IQueryable<User> GetPaginationData(IQueryable<User> query, QueryParams queryParams) {
        return query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }
}
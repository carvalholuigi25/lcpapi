using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface IUsersRepo {
    Task<ActionResult<IEnumerable<User>>> GetUsers(QueryParams queryParams);  
    Task<ActionResult<User>> GetUser(int? id); 
    Task<ActionResult<User>> PostUser(User user);
    Task<IActionResult> PutUser(int? id, User user);
    Task<IActionResult> DeleteUser(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
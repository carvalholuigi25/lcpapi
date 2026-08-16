using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface ISoftwaresRepo {
    Task<ActionResult<IEnumerable<Software>>> GetSoftwares(QueryParams queryParams);  
    Task<ActionResult<Software>> GetSoftware(int? id); 
    Task<ActionResult<Software>> CreateSoftware(Software Software);
    Task<IActionResult> PutSoftware(int? id, Software Software);
    Task<IActionResult> DeleteSoftware(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
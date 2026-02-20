using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface ITvseriesRepo {
    Task<ActionResult<IEnumerable<Tvseries>>> GetTvseries(QueryParams queryParams);  
    Task<IActionResult> GetTvseriesAllInfo();
    Task<IActionResult> GetTvseriesAllInfoById(int? id);
    Task<ActionResult<Tvseries>> GetTvserie(int? id); 
    Task<ActionResult<Tvseries>> CreateTvserie(Tvseries Tvserie);
    Task<IActionResult> PutTvserie(int? id, Tvseries Tvserie);
    Task<IActionResult> DeleteTvserie(int? id);
    Task<IActionResult> DeleteAllTvserie();
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
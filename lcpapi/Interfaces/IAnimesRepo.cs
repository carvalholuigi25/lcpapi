using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface IAnimesRepo {
    Task<ActionResult<IEnumerable<Anime>>> GetAnimes(QueryParams queryParams);  
    Task<ActionResult<Anime>> GetAnime(int? id); 
    Task<ActionResult<Anime>> CreateAnime(Anime Anime);
    Task<IActionResult> PutAnime(int? id, Anime Anime);
    Task<IActionResult> DeleteAnime(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
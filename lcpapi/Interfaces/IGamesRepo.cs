using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface IGamesRepo {
    Task<ActionResult<IEnumerable<Game>>> GetGames(QueryParams queryParams);  
    Task<ActionResult<Game>> GetGame(int? id); 
    Task<ActionResult<Game>> CreateGame(Game Game);
    Task<IActionResult> PutGame(int? id, Game Game);
    Task<IActionResult> DeleteGame(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
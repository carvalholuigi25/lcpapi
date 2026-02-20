using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface IMusicsRepo {
    Task<ActionResult<IEnumerable<Music>>> GetMusics(QueryParams queryParams);  
    Task<ActionResult<Music>> GetMusic(int? id); 
    Task<ActionResult<Music>> CreateMusic(Music Music);
    Task<IActionResult> PutMusic(int? id, Music Music);
    Task<IActionResult> DeleteMusic(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
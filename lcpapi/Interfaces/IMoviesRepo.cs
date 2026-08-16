using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface IMoviesRepo {
    Task<ActionResult<IEnumerable<Movies>>> GetMovies(QueryParams queryParams);  
    Task<ActionResult<Movies>> GetMovie(int? id); 
    Task<ActionResult<Movies>> CreateMovie(Movies Movie);
    Task<IActionResult> PutMovie(int? id, Movies Movie);
    Task<IActionResult> DeleteMovie(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
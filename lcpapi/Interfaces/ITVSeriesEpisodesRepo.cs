using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface ITvseriesEpisodesRepo {
    Task<ActionResult<IEnumerable<TvseriesEpisodesInfo>>> GetTvseriesEpisodes(QueryParams queryParams);
    Task<ActionResult<TvseriesEpisodesInfo>> GetTvserieEpisodes(int? id); 
    Task<ActionResult<TvseriesEpisodesInfo>> CreateTvserieEpisodes(TvseriesEpisodesInfo Tvserie);
    Task<IActionResult> PutTvserieEpisodes(int? id, TvseriesEpisodesInfo Tvserie);
    Task<IActionResult> DeleteTvserieEpisodes(int? id);
    Task<IActionResult> DeleteAllTvserieEpisodes();
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
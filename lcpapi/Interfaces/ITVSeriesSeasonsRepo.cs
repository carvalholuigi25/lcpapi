using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface ITvseriesSeasonsRepo {
    Task<ActionResult<IEnumerable<TvseriesSeasonsInfo>>> GetTvseriesSeasons(QueryParams queryParams);
    Task<ActionResult<TvseriesSeasonsInfo>> GetTvserieSeasons(int? id);
    Task<ActionResult<TvseriesSeasonsInfo>> CreateTvserieSeasons(TvseriesSeasonsInfo Tvserie);
    Task<IActionResult> PutTvserieSeasons(int? id, TvseriesSeasonsInfo Tvserie);
    Task<IActionResult> DeleteTvserieSeasons(int? id);
    Task<IActionResult> DeleteAllTvserieSeasons();
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
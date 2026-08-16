using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface ITvseriesReviewsRepo {
    Task<ActionResult<IEnumerable<TvseriesReviewsInfo>>> GetTvseriesReviews(QueryParams queryParams);
    Task<ActionResult<TvseriesReviewsInfo>> GetTvserieReviews(int? id);
    Task<ActionResult<TvseriesReviewsInfo>> CreateTvserieReviews(TvseriesReviewsInfo Tvserie);
    Task<IActionResult> PutTvserieReviews(int? id, TvseriesReviewsInfo Tvserie);
    Task<IActionResult> DeleteTvserieReviews(int? id);
    Task<IActionResult> DeleteAllTvserieReviews();
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
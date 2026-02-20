using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;

namespace lcpapi.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/tvseries/seasons")]
    [ApiController]
    public class TvseriesSeasonsController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly ITvseriesSeasonsRepo _tvseriesseasonsRepo;

        public TvseriesSeasonsController(ITvseriesSeasonsRepo tvseriesseasonsRepo)
        {
            _tvseriesseasonsRepo = tvseriesseasonsRepo;
        }

        /// <summary>
        /// Gets all tvseriesseasons infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all tvseriesseasons infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about tvseriesseasons</response>
        /// <response code="400">If the tvseriesseasons infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TvseriesSeasonsInfo>>> GetTvseriesSeasons([FromQuery] QueryParams queryParams)
        {
            var tvseriesseasons = await _tvseriesseasonsRepo.GetTvseriesSeasons(queryParams);
            var totalCount = await _tvseriesseasonsRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<TvseriesSeasonsInfo>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = tvseriesseasons.Value!.ToList()
            };

            return Ok(response);

            // return await _context.TvseriesSeasons.ToListAsync();
        }

        /// <summary>
        /// Gets all tvseriesseasons infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all tvseriesseasons infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about tvseriesseasons by id</response>
        /// <response code="400">If the tvseriesseasons infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<TvseriesSeasonsInfo>> GetTvserieSeasons(int id)
        {
            return await _tvseriesseasonsRepo.GetTvserieSeasons(id);

            // var Tvserie = await _context.TvseriesSeasons.FindAsync(id);

            // if (Tvserie == null)
            // {
            //     return NotFound();
            // }

            // return Tvserie;
        }

        /// <summary>
        /// Updates all tvseriesseasons infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Tvserie"></param>
        /// <returns>Updates all tvseriesseasons infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about tvseriesseasons by id</response>
        /// <response code="400">If the updated tvseriesseasons infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTvserieSeasons(int id, TvseriesSeasonsInfo Tvserie)
        {
            return await _tvseriesseasonsRepo.PutTvserieSeasons(id, Tvserie);

            // if (id != Tvserie.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(Tvserie).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!TvserieExists(id))
            //     {
            //         return NotFound();
            //     }
            //     else
            //     {
            //         throw;
            //     }
            // }

            // return NoContent();
        }

        /// <summary>
        /// Creates new tvseriesseasons infos
        /// </summary>
        /// <param name="Tvserie"></param>
        /// <returns>Creates new tvserie info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new tvserie info created</response>
        /// <response code="400">If the new tvserie info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<TvseriesSeasonsInfo>> CreateTvserieSeasons(TvseriesSeasonsInfo Tvserie)
        {
            return await _tvseriesseasonsRepo.CreateTvserieSeasons(Tvserie);

            // _context.TvseriesSeasons.Add(Tvserie);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetTvserie), new { id = Tvserie.Id }, Tvserie);
        }

        /// <summary>
        /// Deletes current tvseriesseasons infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current tvseriesseasons infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted tvserie info</response>
        /// <response code="400">If the specific tvserie info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTvserieSeasons(int id)
        {
            return await _tvseriesseasonsRepo.DeleteTvserieSeasons(id);

            // var Tvserie = await _context.TvseriesSeasons.FindAsync(id);
            // if (Tvserie == null)
            // {
            //     return NotFound();
            // }

            // _context.TvseriesSeasons.Remove(Tvserie);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }

        /// <summary>
        /// Deletes current all tvseriesseasons infos
        /// </summary>
        /// <returns>Deletes current all tvseriesseasons infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all deleted tvseriesseasons infos</response>
        /// <response code="400">If the specific tvserie infos arent deleted</response>
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllTvserieSeasons()
        {
            return await _tvseriesseasonsRepo.DeleteAllTvserieSeasons();
        }
    }
}

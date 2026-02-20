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
    [Route("api/tvseries/episodes")]
    [ApiController]
    public class TvseriesEpisodesController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly ITvseriesEpisodesRepo _tvseriesepisodesRepo;

        public TvseriesEpisodesController(ITvseriesEpisodesRepo tvseriesepisodesRepo)
        {
            _tvseriesepisodesRepo = tvseriesepisodesRepo;
        }

        /// <summary>
        /// Gets all tvseriesepisodes infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all tvseriesepisodes infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about tvseriesepisodes</response>
        /// <response code="400">If the tvseriesepisodes infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TvseriesEpisodesInfo>>> GetTvseriesEpisodes([FromQuery] QueryParams queryParams)
        {
            var tvseriesepisodes = await _tvseriesepisodesRepo.GetTvseriesEpisodes(queryParams);
            var totalCount = await _tvseriesepisodesRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<TvseriesEpisodesInfo>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = tvseriesepisodes.Value!.ToList()
            };

            return Ok(response);

            // return await _context.TvseriesEpisodes.ToListAsync();
        }

        /// <summary>
        /// Gets all tvseriesepisodes infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all tvseriesepisodes infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about tvseriesepisodes by id</response>
        /// <response code="400">If the tvseriesepisodes infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<TvseriesEpisodesInfo>> GetTvserieEpisodes(int id)
        {
            return await _tvseriesepisodesRepo.GetTvserieEpisodes(id);

            // var Tvserie = await _context.TvseriesEpisodes.FindAsync(id);

            // if (Tvserie == null)
            // {
            //     return NotFound();
            // }

            // return Tvserie;
        }

        /// <summary>
        /// Updates all tvseriesepisodes infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Tvserie"></param>
        /// <returns>Updates all tvseriesepisodes infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about tvseriesepisodes by id</response>
        /// <response code="400">If the updated tvseriesepisodes infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTvserieEpisodes(int id, TvseriesEpisodesInfo Tvserie)
        {
            return await _tvseriesepisodesRepo.PutTvserieEpisodes(id, Tvserie);

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
        /// Creates new tvseriesepisodes infos
        /// </summary>
        /// <param name="Tvserie"></param>
        /// <returns>Creates new tvserie info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new tvserie info created</response>
        /// <response code="400">If the new tvserie info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<TvseriesEpisodesInfo>> CreateTvserieEpisodes(TvseriesEpisodesInfo Tvserie)
        {
            return await _tvseriesepisodesRepo.CreateTvserieEpisodes(Tvserie);

            // _context.TvseriesEpisodes.Add(Tvserie);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetTvserie), new { id = Tvserie.Id }, Tvserie);
        }

        /// <summary>
        /// Deletes current tvseriesepisodes infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current tvseriesepisodes infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted tvserie info</response>
        /// <response code="400">If the specific tvserie info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTvserieEpisodes(int id)
        {
            return await _tvseriesepisodesRepo.DeleteTvserieEpisodes(id);

            // var Tvserie = await _context.TvseriesEpisodes.FindAsync(id);
            // if (Tvserie == null)
            // {
            //     return NotFound();
            // }

            // _context.TvseriesEpisodes.Remove(Tvserie);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }

        /// <summary>
        /// Deletes current all tvseriesepisodes infos
        /// </summary>
        /// <returns>Deletes current all tvseriesepisodes infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all deleted tvseriesepisodes infos</response>
        /// <response code="400">If the specific tvserie infos arent deleted</response>
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllTvserieEpisodes()
        {
            return await _tvseriesepisodesRepo.DeleteAllTvserieEpisodes();
        }
    }
}

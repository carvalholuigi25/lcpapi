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
    [Route("api/[controller]")]
    [ApiController]
    public class TvseriesController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly ITvseriesRepo _tvseriesRepo;

        public TvseriesController(ITvseriesRepo tvseriesRepo)
        {
            _tvseriesRepo = tvseriesRepo;
        }

        /// <summary>
        /// Gets all tvseries infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all tvseries infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about tvseries</response>
        /// <response code="400">If the tvseries infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tvseries>>> GetTvseries([FromQuery] QueryParams queryParams)
        {
            var tvseries = await _tvseriesRepo.GetTvseries(queryParams);
            var totalCount = await _tvseriesRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<Tvseries>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = tvseries.Value!.ToList()
            };

            return Ok(response);

            // return await _context.Tvseries.ToListAsync();
        }

        /// <summary>
        /// Gets all tvseries infos with seasons and episodes.
        /// </summary>
        /// <returns>Gets all tvseries infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about tvseries</response>
        /// <response code="400">If the tvseries infos are empty</response>
        [HttpGet("all")]
        public async Task<IActionResult> GetTvseriesAllInfoS()
        {
            return await _tvseriesRepo.GetTvseriesAllInfo();
        }

        /// <summary>
        /// Gets all tvseries infos with seasons and episodes by id.
        /// </summary>
        /// <returns>Gets all tvseries infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about tvseries by id</response>
        /// <response code="400">If the tvseries infos by id are empty</response>
        [HttpGet("all/{id}")]
        public async Task<IActionResult> GetTvseriesAllInfoSById(int id)
        {
            return await _tvseriesRepo.GetTvseriesAllInfoById(id);
        }

        /// <summary>
        /// Gets all tvseries infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all tvseries infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about tvseries by id</response>
        /// <response code="400">If the tvseries infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Tvseries>> GetTvserie(int id)
        {
            return await _tvseriesRepo.GetTvserie(id);

            // var Tvserie = await _context.Tvseries.FindAsync(id);

            // if (Tvserie == null)
            // {
            //     return NotFound();
            // }

            // return Tvserie;
        }

        /// <summary>
        /// Updates all tvseries infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Tvserie"></param>
        /// <returns>Updates all tvseries infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about tvseries by id</response>
        /// <response code="400">If the updated tvseries infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTvserie(int id, Tvseries Tvserie)
        {
            return await _tvseriesRepo.PutTvserie(id, Tvserie);

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
        /// Creates new tvseries infos
        /// </summary>
        /// <param name="Tvserie"></param>
        /// <returns>Creates new tvserie info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new tvserie info created</response>
        /// <response code="400">If the new tvserie info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<Tvseries>> CreateTvserie(Tvseries Tvserie)
        {
            return await _tvseriesRepo.CreateTvserie(Tvserie);

            // _context.Tvseries.Add(Tvserie);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetTvserie), new { id = Tvserie.Id }, Tvserie);
        }

        /// <summary>
        /// Deletes current tvseries infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current tvseries infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted tvserie info</response>
        /// <response code="400">If the specific tvserie info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTvserie(int id)
        {
            return await _tvseriesRepo.DeleteTvserie(id);

            // var Tvserie = await _context.Tvseries.FindAsync(id);
            // if (Tvserie == null)
            // {
            //     return NotFound();
            // }

            // _context.Tvseries.Remove(Tvserie);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }

        /// <summary>
        /// Deletes current all tvseries infos
        /// </summary>
        /// <returns>Deletes current all tvseries infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all deleted tvseries infos</response>
        /// <response code="400">If the specific tvserie infos arent deleted</response>
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllTvserie()
        {
            return await _tvseriesRepo.DeleteAllTvserie();
        }
    }
}

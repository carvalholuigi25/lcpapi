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
    public class AnimesController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly IAnimesRepo _animesRepo;

        public AnimesController(IAnimesRepo animesRepo)
        {
            _animesRepo = animesRepo;
        }

        /// <summary>
        /// Gets all animes infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all animes infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about animes</response>
        /// <response code="400">If the animes infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Anime>>> GetAnimes([FromQuery] QueryParams queryParams)
        {
            var animes = await _animesRepo.GetAnimes(queryParams);
            var totalCount = await _animesRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<Anime>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = animes.Value!.ToList()
            };

            return Ok(response);

            // return await _context.Animes.ToListAsync();
        }

        /// <summary>
        /// Gets all animes infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all animes infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about animes by id</response>
        /// <response code="400">If the animes infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Anime>> GetAnime(int id)
        {
            return await _animesRepo.GetAnime(id);

            // var Anime = await _context.Animes.FindAsync(id);

            // if (Anime == null)
            // {
            //     return NotFound();
            // }

            // return Anime;
        }

        /// <summary>
        /// Updates all animes infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Anime"></param>
        /// <returns>Updates all animes infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about animes by id</response>
        /// <response code="400">If the updated animes infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnime(int id, Anime Anime)
        {
            return await _animesRepo.PutAnime(id, Anime);

            // if (id != Anime.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(Anime).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!AnimeExists(id))
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
        /// Creates new animes infos
        /// </summary>
        /// <param name="Anime"></param>
        /// <returns>Creates new anime info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new anime info created</response>
        /// <response code="400">If the new anime info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<Anime>> CreateAnime(Anime Anime)
        {
            return await _animesRepo.CreateAnime(Anime);

            // _context.Animes.Add(Anime);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetAnime), new { id = Anime.Id }, Anime);
        }

        /// <summary>
        /// Deletes current animes infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current animes infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted anime info</response>
        /// <response code="400">If the specific anime info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnime(int id)
        {
            return await _animesRepo.DeleteAnime(id);

            // var Anime = await _context.Animes.FindAsync(id);
            // if (Anime == null)
            // {
            //     return NotFound();
            // }

            // _context.Animes.Remove(Anime);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }
    }
}

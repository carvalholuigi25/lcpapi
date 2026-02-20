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
    public class MusicsController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly IMusicsRepo _musicsRepo;

        public MusicsController(IMusicsRepo musicsRepo)
        {
            _musicsRepo = musicsRepo;
        }

        /// <summary>
        /// Gets all musics infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all musics infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about musics</response>
        /// <response code="400">If the musics infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Music>>> GetMusics([FromQuery] QueryParams queryParams)
        {
            var musics = await _musicsRepo.GetMusics(queryParams);
            var totalCount = await _musicsRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<Music>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = musics.Value!.ToList()
            };

            return Ok(response);

            // return await _context.Musics.ToListAsync();
        }

        /// <summary>
        /// Gets all musics infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all musics infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about musics by id</response>
        /// <response code="400">If the musics infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Music>> GetMusic(int id)
        {
            return await _musicsRepo.GetMusic(id);

            // var Music = await _context.Musics.FindAsync(id);

            // if (Music == null)
            // {
            //     return NotFound();
            // }

            // return Music;
        }

        /// <summary>
        /// Updates all musics infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Music"></param>
        /// <returns>Updates all musics infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about musics by id</response>
        /// <response code="400">If the updated musics infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMusic(int id, Music Music)
        {
            return await _musicsRepo.PutMusic(id, Music);

            // if (id != Music.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(Music).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!MusicExists(id))
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
        /// Creates new musics infos
        /// </summary>
        /// <param name="Music"></param>
        /// <returns>Creates new music info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new music info created</response>
        /// <response code="400">If the new music info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<Music>> CreateMusic(Music Music)
        {
            return await _musicsRepo.CreateMusic(Music);

            // _context.Musics.Add(Music);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetMusic), new { id = Music.Id }, Music);
        }

        /// <summary>
        /// Deletes current musics infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current musics infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted music info</response>
        /// <response code="400">If the specific music info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMusic(int id)
        {
            return await _musicsRepo.DeleteMusic(id);

            // var Music = await _context.Musics.FindAsync(id);
            // if (Music == null)
            // {
            //     return NotFound();
            // }

            // _context.Musics.Remove(Music);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }
    }
}

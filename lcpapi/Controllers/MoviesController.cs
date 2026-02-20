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
    public class MoviesController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly IMoviesRepo _moviesRepo;

        public MoviesController(IMoviesRepo moviesRepo)
        {
            _moviesRepo = moviesRepo;
        }

        /// <summary>
        /// Gets all movies infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all movies infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about movies</response>
        /// <response code="400">If the movies infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movies>>> GetMovies([FromQuery] QueryParams queryParams)
        {
            var movies = await _moviesRepo.GetMovies(queryParams);
            var totalCount = await _moviesRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<Movies>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = movies.Value!.ToList()
            };

            return Ok(response);

            // return await _context.Movies.ToListAsync();
        }

        /// <summary>
        /// Gets all movies infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all movies infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about movies by id</response>
        /// <response code="400">If the movies infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Movies>> GetMovie(int id)
        {
            return await _moviesRepo.GetMovie(id);

            // var Movie = await _context.Movies.FindAsync(id);

            // if (Movie == null)
            // {
            //     return NotFound();
            // }

            // return Movie;
        }

        /// <summary>
        /// Updates all movies infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Movie"></param>
        /// <returns>Updates all movies infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about movies by id</response>
        /// <response code="400">If the updated movies infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movies Movie)
        {
            return await _moviesRepo.PutMovie(id, Movie);

            // if (id != Movie.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(Movie).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!MovieExists(id))
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
        /// Creates new movies infos
        /// </summary>
        /// <param name="Movie"></param>
        /// <returns>Creates new movie info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new movie info created</response>
        /// <response code="400">If the new movie info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<Movies>> CreateMovie(Movies Movie)
        {
            return await _moviesRepo.CreateMovie(Movie);

            // _context.Movies.Add(Movie);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetMovie), new { id = Movie.Id }, Movie);
        }

        /// <summary>
        /// Deletes current movies infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current movies infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted movie info</response>
        /// <response code="400">If the specific movie info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            return await _moviesRepo.DeleteMovie(id);

            // var Movie = await _context.Movies.FindAsync(id);
            // if (Movie == null)
            // {
            //     return NotFound();
            // }

            // _context.Movies.Remove(Movie);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }
    }
}

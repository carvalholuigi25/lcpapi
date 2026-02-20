using Microsoft.AspNetCore.Mvc;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using lcpapi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace lcpapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly IGamesRepo _gamesRepo;

        public GamesController(IGamesRepo gamesRepo, IHubContext<ChatHub> hubContext)
        {
            _gamesRepo = gamesRepo;
        }

        /// <summary>
        /// Gets all games infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all games infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about games</response>
        /// <response code="400">If the games infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames([FromQuery] QueryParams queryParams)
        {
            var games = await _gamesRepo.GetGames(queryParams);
            var totalCount = await _gamesRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<Game>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = games.Value!.ToList()
            };

            return Ok(response);

            // return await _context.Games.ToListAsync();
        }

        /// <summary>
        /// Gets all games infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all games infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about games by id</response>
        /// <response code="400">If the games infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
            return await _gamesRepo.GetGame(id);

            // var Game = await _context.Games.FindAsync(id);

            // if (Game == null)
            // {
            //     return NotFound();
            // }

            // return Game;
        }

        /// <summary>
        /// Updates all games infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Game"></param>
        /// <returns>Updates all games infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about games by id</response>
        /// <response code="400">If the updated games infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, Game Game)
        {
            return await _gamesRepo.PutGame(id, Game);

            // if (id != Game.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(Game).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!GameExists(id))
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
        /// Creates new games infos
        /// </summary>
        /// <param name="Game"></param>
        /// <returns>Creates new game info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new game info created</response>
        /// <response code="400">If the new game info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<Game>> CreateGame(Game Game)
        {
            return await _gamesRepo.CreateGame(Game);

            // _context.Games.Add(Game);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetGame), new { id = Game.Id }, Game);
        }

        /// <summary>
        /// Deletes current games infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current games infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted game info</response>
        /// <response code="400">If the specific game info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            return await _gamesRepo.DeleteGame(id);

            // var Game = await _context.Games.FindAsync(id);
            // if (Game == null)
            // {
            //     return NotFound();
            // }

            // _context.Games.Remove(Game);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }
    }
}

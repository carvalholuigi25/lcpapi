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
    public class ActionFiguresController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly IActionFiguresRepo _actionfiguresRepo;

        public ActionFiguresController(IActionFiguresRepo actionfiguresRepo)
        {
            _actionfiguresRepo = actionfiguresRepo;
        }

        /// <summary>
        /// Gets all actionfigures infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all actionfigures infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about actionfigures</response>
        /// <response code="400">If the actionfigures infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActionFigure>>> GetActionFigures([FromQuery] QueryParams queryParams)
        {
            var actionfigures = await _actionfiguresRepo.GetActionFigures(queryParams);
            var totalCount = await _actionfiguresRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<ActionFigure>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = actionfigures.Value!.ToList()
            };

            return Ok(response);

            // return await _context.ActionFigures.ToListAsync();
        }

        /// <summary>
        /// Gets all actionfigures infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all actionfigures infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about actionfigures by id</response>
        /// <response code="400">If the actionfigures infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ActionFigure>> GetActionFigure(int id)
        {
            return await _actionfiguresRepo.GetActionFigure(id);

            // var ActionFigure = await _context.ActionFigures.FindAsync(id);

            // if (ActionFigure == null)
            // {
            //     return NotFound();
            // }

            // return ActionFigure;
        }

        /// <summary>
        /// Updates all actionfigures infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ActionFigure"></param>
        /// <returns>Updates all actionfigures infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about actionfigures by id</response>
        /// <response code="400">If the updated actionfigures infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActionFigure(int id, ActionFigure ActionFigure)
        {
            return await _actionfiguresRepo.PutActionFigure(id, ActionFigure);

            // if (id != ActionFigure.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(ActionFigure).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!ActionFigureExists(id))
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
        /// Creates new actionfigures infos
        /// </summary>
        /// <param name="ActionFigure"></param>
        /// <returns>Creates new actionfigure info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new actionfigure info created</response>
        /// <response code="400">If the new actionfigure info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<ActionFigure>> CreateActionFigure(ActionFigure ActionFigure)
        {
            return await _actionfiguresRepo.CreateActionFigure(ActionFigure);

            // _context.ActionFigures.Add(ActionFigure);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetActionFigure), new { id = ActionFigure.Id }, ActionFigure);
        }

        /// <summary>
        /// Deletes current actionfigures infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current actionfigures infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted actionfigure info</response>
        /// <response code="400">If the specific actionfigure info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActionFigure(int id)
        {
            return await _actionfiguresRepo.DeleteActionFigure(id);

            // var ActionFigure = await _context.ActionFigures.FindAsync(id);
            // if (ActionFigure == null)
            // {
            //     return NotFound();
            // }

            // _context.ActionFigures.Remove(ActionFigure);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }
    }
}

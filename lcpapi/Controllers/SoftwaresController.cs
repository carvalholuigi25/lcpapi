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
    public class SoftwaresController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly ISoftwaresRepo _softwaresRepo;

        public SoftwaresController(ISoftwaresRepo softwaresRepo)
        {
            _softwaresRepo = softwaresRepo;
        }

        /// <summary>
        /// Gets all softwares infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all softwares infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about softwares</response>
        /// <response code="400">If the softwares infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Software>>> GetSoftwares([FromQuery] QueryParams queryParams)
        {
            var softwares = await _softwaresRepo.GetSoftwares(queryParams);
            var totalCount = await _softwaresRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<Software>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = softwares.Value!.ToList()
            };

            return Ok(response);

            // return await _context.Softwares.ToListAsync();
        }

        /// <summary>
        /// Gets all softwares infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all softwares infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about softwares by id</response>
        /// <response code="400">If the softwares infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Software>> GetSoftware(int id)
        {
            return await _softwaresRepo.GetSoftware(id);

            // var Software = await _context.Softwares.FindAsync(id);

            // if (Software == null)
            // {
            //     return NotFound();
            // }

            // return Software;
        }

        /// <summary>
        /// Updates all softwares infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Software"></param>
        /// <returns>Updates all softwares infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about softwares by id</response>
        /// <response code="400">If the updated softwares infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSoftware(int id, Software Software)
        {
            return await _softwaresRepo.PutSoftware(id, Software);

            // if (id != Software.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(Software).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!SoftwareExists(id))
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
        /// Creates new softwares infos
        /// </summary>
        /// <param name="Software"></param>
        /// <returns>Creates new software info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new software info created</response>
        /// <response code="400">If the new software info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<Software>> CreateSoftware(Software Software)
        {
            return await _softwaresRepo.CreateSoftware(Software);

            // _context.Softwares.Add(Software);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetSoftware), new { id = Software.Id }, Software);
        }

        /// <summary>
        /// Deletes current softwares infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current softwares infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted software info</response>
        /// <response code="400">If the specific software info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSoftware(int id)
        {
            return await _softwaresRepo.DeleteSoftware(id);

            // var Software = await _context.Softwares.FindAsync(id);
            // if (Software == null)
            // {
            //     return NotFound();
            // }

            // _context.Softwares.Remove(Software);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }
    }
}

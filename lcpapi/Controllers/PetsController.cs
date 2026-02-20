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
    public class PetController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly IPetsRepo _petRepo;

        public PetController(IPetsRepo petRepo)
        {
            _petRepo = petRepo;
        }

        /// <summary>
        /// Gets all pet infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all pet infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about pet</response>
        /// <response code="400">If the pet infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPet([FromQuery] QueryParams queryParams)
        {
            var pet = await _petRepo.GetPets(queryParams);
            var totalCount = await _petRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<Pet>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = pet.Value!.ToList()
            };

            return Ok(response);

            // return await _context.Pet.ToListAsync();
        }

        /// <summary>
        /// Gets all pet infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all pet infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about pet by id</response>
        /// <response code="400">If the pet infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetPet(int id)
        {
            return await _petRepo.GetPet(id);

            // var Pet = await _context.Pet.FindAsync(id);

            // if (Pet == null)
            // {
            //     return NotFound();
            // }

            // return Pet;
        }

        /// <summary>
        /// Updates all pet infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Pet"></param>
        /// <returns>Updates all pet infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about pet by id</response>
        /// <response code="400">If the updated pet infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPet(int id, Pet Pet)
        {
            return await _petRepo.PutPet(id, Pet);

            // if (id != Pet.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(Pet).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!PetExists(id))
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
        /// Creates new pet infos
        /// </summary>
        /// <param name="Pet"></param>
        /// <returns>Creates new pet info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new pet info created</response>
        /// <response code="400">If the new pet info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<Pet>> CreatePet(Pet Pet)
        {
            return await _petRepo.CreatePet(Pet);

            // _context.Pet.Add(Pet);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetPet), new { id = Pet.Id }, Pet);
        }

        /// <summary>
        /// Deletes current pet infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current pet infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted pet info</response>
        /// <response code="400">If the specific pet info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            return await _petRepo.DeletePet(id);

            // var Pet = await _context.Pet.FindAsync(id);
            // if (Pet == null)
            // {
            //     return NotFound();
            // }

            // _context.Pet.Remove(Pet);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }
    }
}

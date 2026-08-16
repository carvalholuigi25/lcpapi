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
    public class RecipesFoodsController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly IRecipesFoodsRepo _recipesfoodsRepo;

        public RecipesFoodsController(IRecipesFoodsRepo recipesfoodsRepo)
        {
            _recipesfoodsRepo = recipesfoodsRepo;
        }

        /// <summary>
        /// Gets all recipesfoods infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all recipesfoods infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about recipesfoods</response>
        /// <response code="400">If the recipesfoods infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipesFoods>>> GetRecipesFoods([FromQuery] QueryParams queryParams)
        {
            var recipesfoods = await _recipesfoodsRepo.GetRecipesFoods(queryParams);
            var totalCount = await _recipesfoodsRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<RecipesFoods>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = recipesfoods.Value!.ToList()
            };

            return Ok(response);

            // return await _context.RecipesFoods.ToListAsync();
        }

        /// <summary>
        /// Gets all recipesfoods infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all recipesfoods infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about recipesfoods by id</response>
        /// <response code="400">If the recipesfoods infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipesFoods>> GetRecipesFood(int id)
        {
            return await _recipesfoodsRepo.GetRecipesFood(id);

            // var RecipesFood = await _context.RecipesFoods.FindAsync(id);

            // if (RecipesFood == null)
            // {
            //     return NotFound();
            // }

            // return RecipesFood;
        }

        /// <summary>
        /// Updates all recipesfoods infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="RecipesFood"></param>
        /// <returns>Updates all recipesfoods infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about recipesfoods by id</response>
        /// <response code="400">If the updated recipesfoods infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipesFood(int id, RecipesFoods RecipesFood)
        {
            return await _recipesfoodsRepo.PutRecipesFoods(id, RecipesFood);

            // if (id != RecipesFood.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(RecipesFood).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!RecipesFoodExists(id))
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
        /// Creates new recipesfoods infos
        /// </summary>
        /// <param name="RecipesFood"></param>
        /// <returns>Creates new recipesfood info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new recipesfood info created</response>
        /// <response code="400">If the new recipesfood info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<RecipesFoods>> CreateRecipesFood(RecipesFoods RecipesFood)
        {
            return await _recipesfoodsRepo.CreateRecipesFoods(RecipesFood);

            // _context.RecipesFoods.Add(RecipesFood);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetRecipesFood), new { id = RecipesFood.Id }, RecipesFood);
        }

        /// <summary>
        /// Deletes current recipesfoods infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current recipesfoods infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted recipesfood info</response>
        /// <response code="400">If the specific recipesfood info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipesFood(int id)
        {
            return await _recipesfoodsRepo.DeleteRecipesFoods(id);

            // var RecipesFood = await _context.RecipesFoods.FindAsync(id);
            // if (RecipesFood == null)
            // {
            //     return NotFound();
            // }

            // _context.RecipesFoods.Remove(RecipesFood);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }
    }
}

using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface IRecipesFoodsRepo {
    Task<ActionResult<IEnumerable<RecipesFoods>>> GetRecipesFoods(QueryParams queryParams);  
    Task<ActionResult<RecipesFoods>> GetRecipesFood(int? id); 
    Task<ActionResult<RecipesFoods>> CreateRecipesFoods(RecipesFoods RecipeFood);
    Task<IActionResult> PutRecipesFoods(int? id, RecipesFoods RecipeFood);
    Task<IActionResult> DeleteRecipesFoods(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface IPetsRepo {
    Task<ActionResult<IEnumerable<Pet>>> GetPets(QueryParams queryParams);  
    Task<ActionResult<Pet>> GetPet(int? id); 
    Task<ActionResult<Pet>> CreatePet(Pet Pet);
    Task<IActionResult> PutPet(int? id, Pet Pet);
    Task<IActionResult> DeletePet(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
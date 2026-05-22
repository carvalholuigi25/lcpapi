using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface ISettingsRepo {
    Task<ActionResult<IEnumerable<Settings>>> GetSettings(QueryParams queryParams);  
    Task<ActionResult<Settings>> GetSetting(int? id); 
    Task<ActionResult<Settings>> PostSetting(Settings setting);
    Task<IActionResult> PutSetting(int? id, Settings setting);
    Task<IActionResult> DeleteSetting(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface IActionFiguresRepo {
    Task<ActionResult<IEnumerable<ActionFigure>>> GetActionFigures(QueryParams queryParams);  
    Task<ActionResult<ActionFigure>> GetActionFigure(int? id); 
    Task<ActionResult<ActionFigure>> CreateActionFigure(ActionFigure ActionFigure);
    Task<IActionResult> PutActionFigure(int? id, ActionFigure ActionFigure);
    Task<IActionResult> DeleteActionFigure(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
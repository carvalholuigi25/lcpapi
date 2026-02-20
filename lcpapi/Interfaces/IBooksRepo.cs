using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface IBooksRepo {
    Task<ActionResult<IEnumerable<Book>>> GetBooks(QueryParams queryParams);  
    Task<ActionResult<Book>> GetBook(int? id); 
    Task<ActionResult<Book>> CreateBook(Book Book);
    Task<IActionResult> PutBook(int? id, Book Book);
    Task<IActionResult> DeleteBook(int? id);
    Task<int> GetTotalCountAsync(QueryParams queryParams);
}
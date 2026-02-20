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
    public class BooksController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly IBooksRepo _booksRepo;

        public BooksController(IBooksRepo booksRepo)
        {
            _booksRepo = booksRepo;
        }

        /// <summary>
        /// Gets all books infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all books infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about books</response>
        /// <response code="400">If the books infos are empty</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] QueryParams queryParams)
        {
            var books = await _booksRepo.GetBooks(queryParams);
            var totalCount = await _booksRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<Book>
            {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = books.Value!.ToList()
            };

            return Ok(response);

            // return await _context.Books.ToListAsync();
        }

        /// <summary>
        /// Gets all books infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets all books infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the all infos about books by id</response>
        /// <response code="400">If the books infos by id are empty</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            return await _booksRepo.GetBook(id);

            // var Book = await _context.Books.FindAsync(id);

            // if (Book == null)
            // {
            //     return NotFound();
            // }

            // return Book;
        }

        /// <summary>
        /// Updates all books infos by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Book"></param>
        /// <returns>Updates all books infos by id</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the updated infos about books by id</response>
        /// <response code="400">If the updated books infos by id are empty</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book Book)
        {
            return await _booksRepo.PutBook(id, Book);

            // if (id != Book.Id)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(Book).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!BookExists(id))
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
        /// Creates new books infos
        /// </summary>
        /// <param name="Book"></param>
        /// <returns>Creates new book info</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the new book info created</response>
        /// <response code="400">If the new book info isnt created</response>
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book Book)
        {
            return await _booksRepo.CreateBook(Book);

            // _context.Books.Add(Book);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetBook), new { id = Book.Id }, Book);
        }

        /// <summary>
        /// Deletes current books infos
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes current books infos</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the deleted book info</response>
        /// <response code="400">If the specific book info isnt deleted</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            return await _booksRepo.DeleteBook(id);

            // var Book = await _context.Books.FindAsync(id);
            // if (Book == null)
            // {
            //     return NotFound();
            // }

            // _context.Books.Remove(Book);
            // await _context.SaveChangesAsync();

            // return NoContent();
        }
    }
}

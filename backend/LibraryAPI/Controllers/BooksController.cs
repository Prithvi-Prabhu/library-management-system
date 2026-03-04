using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

public class BooksController : ODataController
{
    private readonly LibraryContext _context;
    public BooksController(LibraryContext context) { _context = context; }

    // GET /odata/Books  — supports ?$filter, ?$orderby, ?$top etc
    [EnableQuery]
    public IQueryable<Book> Get() => _context.Books;

    // GET /odata/Books(1)
    [EnableQuery]
    public SingleResult<Book> Get(int key) =>
        SingleResult.Create(_context.Books.Where(b => b.Id == key));

    // POST /odata/Books
    public async Task<IActionResult> Post([FromBody] Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return Created(book);
    }

    // PATCH /odata/Books(1)
    public async Task<IActionResult> Patch(int key, [FromBody] Book update)
    {
        var book = await _context.Books.FindAsync(key);
        if (book == null) return NotFound();
        book.Title = update.Title ?? book.Title;
        book.AvailableCopies = update.AvailableCopies;
        await _context.SaveChangesAsync();
        return Updated(book);
    }

    // DELETE /odata/Books(1)
    public async Task<IActionResult> Delete(int key)
    {
        var book = await _context.Books.FindAsync(key);
        if (book == null) return NotFound();
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

public class LoansController : ODataController
{
    private readonly LibraryContext _context;

    public LoansController(LibraryContext context)
    {
        _context = context;
    }

    // GET /odata/Loans
    [EnableQuery]
    public IQueryable<Loan> Get()
    {
        return _context.Loans;
    }

    // GET /odata/Loans(1)
    [EnableQuery]
    public SingleResult<Loan> Get(int key)
    {
        return SingleResult.Create(
            _context.Loans.Where(l => l.Id == key)
        );
    }

    // POST /odata/Loans  (Borrow a book)
    public async Task<IActionResult> Post([FromBody] Loan loan)
    {
        if (loan == null)
            return BadRequest("Loan data is required.");

        // Validate Book
        var book = await _context.Books.FindAsync(loan.BookId);
        if (book == null)
            return BadRequest("Invalid BookId.");

        // Validate Member
        var member = await _context.Members.FindAsync(loan.MemberId);
        if (member == null)
            return BadRequest("Invalid MemberId.");

        // Check availability
        if (book.AvailableCopies < 1)
            return BadRequest("No copies available.");

        // Set business logic values (server-controlled)
        loan.LoanDate = DateTime.UtcNow;
        loan.DueDate = DateTime.UtcNow.AddDays(14); // 2 weeks
        loan.Returned = false;

        // Reduce available copies
        book.AvailableCopies--;

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();

        return Created(loan);
    }

    // PATCH /odata/Loans(1)  (Mark as returned)
    public async Task<IActionResult> Patch(int key, [FromBody] Loan update)
    {
        var loan = await _context.Loans.FindAsync(key);
        if (loan == null)
            return NotFound();

        if (update == null)
            return BadRequest("Update data required.");

        // If marking returned
        if (update.Returned && !loan.Returned)
        {
            var book = await _context.Books.FindAsync(loan.BookId);
            if (book != null)
                book.AvailableCopies++;
        }

        loan.Returned = update.Returned;

        await _context.SaveChangesAsync();
        return Updated(loan);
    }

    // DELETE /odata/Loans(1)
    public async Task<IActionResult> Delete(int key)
    {
        var loan = await _context.Loans.FindAsync(key);
        if (loan == null)
            return NotFound();

        _context.Loans.Remove(loan);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Controllers
{
    public class MembersController : ODataController
    {
        private readonly LibraryContext _context;

        public MembersController(LibraryContext context)
        {
            _context = context;
        }

        // GET /odata/Members
        [EnableQuery]
        public IQueryable<Member> Get()
        {
            return _context.Members;
        }

        // GET /odata/Members(1)
        [EnableQuery]
        public SingleResult<Member> Get(int key)
        {
            return SingleResult.Create(
                _context.Members.Where(m => m.Id == key)
            );
        }

        // POST /odata/Members
        public async Task<IActionResult> Post([FromBody] Member member)
        {
            if (member == null)
                return BadRequest("Invalid member data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Automatically set join date
            member.JoinDate = DateTime.UtcNow;

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return Created(member);
        }

        // PATCH /odata/Members(1)
        public async Task<IActionResult> Patch(int key, [FromBody] Member update)
        {
            var member = await _context.Members.FindAsync(key);
            if (member == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(update.Name))
                member.Name = update.Name;

            if (!string.IsNullOrWhiteSpace(update.Email))
                member.Email = update.Email;

            await _context.SaveChangesAsync();

            return Updated(member);
        }

        // DELETE /odata/Members(1)
        public async Task<IActionResult> Delete(int key)
        {
            var member = await _context.Members
                .Include(m => m.Loans)
                .FirstOrDefaultAsync(m => m.Id == key);

            if (member == null)
                return NotFound();

            // Prevent deleting member with active (not returned) loans
            if (member.Loans != null && member.Loans.Any(l => !l.Returned))
                return BadRequest("Member has active loans and cannot be deleted.");

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

using LibraryAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace LibraryAPI.Data
{
  
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }
    }
}

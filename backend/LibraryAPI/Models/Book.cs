using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        public string? Genre { get; set; }

        [Range(1000, 2100)]
        public int YearPublished { get; set; }

        [Range(0, 1000)]
        public int AvailableCopies { get; set; }

        public ICollection<Loan>? Loans { get; set; }
    }
}
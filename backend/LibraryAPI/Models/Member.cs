using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime JoinDate { get; set; }

        public ICollection<Loan>? Loans { get; set; }
    }
}
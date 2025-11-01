using System.ComponentModel.DataAnnotations;

namespace BlazorLibraryApp.Models
{
    public class BookCategory
    {
        [Required]
        public int BookId { get; set; }
        public Book? Book { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
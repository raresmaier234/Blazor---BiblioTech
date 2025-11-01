using System.ComponentModel.DataAnnotations;

namespace BlazorLibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul cărții este obligatoriu")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Titlul trebuie să aibă între 1 și 200 de caractere")]
        public required string Title { get; set; }

        [Range(1000, 9999, ErrorMessage = "Anul trebuie să fie între 1000 și 9999")]
        [Display(Name = "Anul publicării")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Autorul este obligatoriu")]
        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<BookCategory> BookCategories { get; set; } = new();
    }
}
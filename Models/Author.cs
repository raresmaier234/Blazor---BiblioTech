using System.ComponentModel.DataAnnotations;

namespace BlazorLibraryApp.Models
{
    public class Author
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Numele autorului este obligatoriu")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Numele trebuie să aibă între 2 și 100 de caractere")]
        public required string Name { get; set; }
        
        [Required(ErrorMessage = "Adresa de email este obligatorie")]
        [EmailAddress(ErrorMessage = "Formatul emailului nu este valid")]
        [StringLength(255, ErrorMessage = "Emailul nu poate avea mai mult de 255 de caractere")]
        public required string Email { get; set; }
        
        [StringLength(1000, ErrorMessage = "Biografia nu poate avea mai mult de 1000 de caractere")]
        public string? Biography { get; set; }

        public List<Book> Books { get; set; } = new();
    }
}
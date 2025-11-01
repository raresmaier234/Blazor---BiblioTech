using System.ComponentModel.DataAnnotations;

namespace BlazorLibraryApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Numele trebuie să aibă între 2 și 100 de caractere")]
        public required string Name { get; set; }
        
        [StringLength(500, ErrorMessage = "Descrierea nu poate avea mai mult de 500 de caractere")]
        public string? Description { get; set; }
        
        public List<BookCategory> BookCategories { get; set; } = new();
    }
}
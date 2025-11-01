using Microsoft.EntityFrameworkCore;
using BlazorLibraryApp.Models;

namespace BlazorLibraryApp.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Folosim SQLite, baza de date va fi creată în folderul proiectului
            optionsBuilder.UseSqlite("Data Source=library.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurare many-to-many Book <-> Category
            modelBuilder.Entity<BookCategory>()
                .HasKey(cc => new { cc.BookId, cc.CategoryId });

            modelBuilder.Entity<BookCategory>()
                .HasOne(cc => cc.Book)
                .WithMany(c => c.BookCategories)
                .HasForeignKey(cc => cc.BookId);

            modelBuilder.Entity<BookCategory>()
                .HasOne(cc => cc.Category)
                .WithMany(c => c.BookCategories)
                .HasForeignKey(cc => cc.CategoryId);

            // Configurare one-to-many Author -> Book
            modelBuilder.Entity<Book>()
                .HasOne(c => c.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(c => c.AuthorId);
        }
    }
}

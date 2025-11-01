using BlazorLibraryApp.Data;
using BlazorLibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorLibraryApp.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly LibraryContext _context;

        public AuthorService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors
                .Include(a => a.Books)
                .ToListAsync();
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Author> CreateAuthorAsync(Author author)
        {
            // Validare strictă pentru a preveni crearea autorilor fără date
            if (string.IsNullOrWhiteSpace(author.Name) || author.Name.Trim().Length < 2)
            {
                throw new InvalidOperationException("Numele autorului trebuie să aibă cel puțin 2 caractere.");
            }

            if (string.IsNullOrWhiteSpace(author.Email) || !author.Email.Contains("@"))
            {
                throw new InvalidOperationException("Este necesar un email valid pentru autor.");
            }

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<Author?> UpdateAuthorAsync(int id, Author author)
        {
            var existingAuthor = await _context.Authors.FindAsync(id);
            if (existingAuthor == null)
                return null;

            // Validare strictă
            if (string.IsNullOrWhiteSpace(author.Name) || author.Name.Trim().Length < 2)
            {
                throw new InvalidOperationException("Numele autorului trebuie să aibă cel puțin 2 caractere.");
            }

            if (string.IsNullOrWhiteSpace(author.Email) || !author.Email.Contains("@"))
            {
                throw new InvalidOperationException("Este necesar un email valid pentru autor.");
            }

            existingAuthor.Name = author.Name;
            existingAuthor.Email = author.Email;
            existingAuthor.Biography = author.Biography;

            await _context.SaveChangesAsync();
            return existingAuthor;
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return false;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
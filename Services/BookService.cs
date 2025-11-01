using BlazorLibraryApp.Data;
using BlazorLibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorLibraryApp.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryContext _context;

        public BookService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book?> UpdateBookAsync(int id, Book book)
        {
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
                return null;

            existingBook.Title = book.Title;
            existingBook.Year = book.Year;
            existingBook.AuthorId = book.AuthorId;

            await _context.SaveChangesAsync();
            return existingBook;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Book> CreateBookWithCategoriesAsync(Book book, List<int> categoryIds)
        {
            // Verifică că autorul există
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == book.AuthorId);
            if (!authorExists)
            {
                throw new InvalidOperationException($"Autorul cu ID-ul {book.AuthorId} nu există.");
            }

            // Obține ora din București (Romania)
            var romanianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Bucharest");
            var romanianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, romanianTimeZone);

            // Nu salvăm obiectul Author nested - doar AuthorId
            var bookToAdd = new Book
            {
                Title = book.Title,
                Year = book.Year,
                AuthorId = book.AuthorId,
                CreatedAt = romanianTime
            };

            _context.Books.Add(bookToAdd);
            await _context.SaveChangesAsync();

            // Adaugă categoriile
            foreach (var categoryId in categoryIds)
            {
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);
                if (categoryExists)
                {
                    var bookCategory = new BookCategory
                    {
                        BookId = bookToAdd.Id,
                        CategoryId = categoryId
                    };
                    _context.BookCategories.Add(bookCategory);
                }
            }

            await _context.SaveChangesAsync();
            return await GetBookByIdAsync(bookToAdd.Id) ?? bookToAdd;
        }

        public async Task<Book?> UpdateBookWithCategoriesAsync(int id, Book book, List<int> categoryIds)
        {
            var existingBook = await _context.Books
                .Include(b => b.BookCategories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBook == null)
                return null;

            // Verifică că autorul există
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == book.AuthorId);
            if (!authorExists)
            {
                throw new InvalidOperationException($"Autorul cu ID-ul {book.AuthorId} nu există.");
            }

            existingBook.Title = book.Title;
            existingBook.Year = book.Year;
            existingBook.AuthorId = book.AuthorId;

            // Șterge categoriile existente
            _context.BookCategories.RemoveRange(existingBook.BookCategories);

            // Adaugă categoriile noi
            foreach (var categoryId in categoryIds)
            {
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);
                if (categoryExists)
                {
                    var bookCategory = new BookCategory
                    {
                        BookId = existingBook.Id,
                        CategoryId = categoryId
                    };
                    _context.BookCategories.Add(bookCategory);
                }
            }

            await _context.SaveChangesAsync();
            return await GetBookByIdAsync(existingBook.Id);
        }

        public async Task<List<Book>> GetRecentBooksAsync(int count = 3)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .OrderByDescending(b => b.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
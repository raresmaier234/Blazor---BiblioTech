using BlazorLibraryApp.Models;

namespace BlazorLibraryApp.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> CreateBookAsync(Book book);
        Task<Book?> UpdateBookAsync(int id, Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<Book> CreateBookWithCategoriesAsync(Book book, List<int> categoryIds);
        Task<Book?> UpdateBookWithCategoriesAsync(int id, Book book, List<int> categoryIds);
        Task<List<Book>> GetRecentBooksAsync(int count = 3);
    }
}
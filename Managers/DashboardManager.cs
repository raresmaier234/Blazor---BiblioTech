using BlazorLibraryApp.Services;
using BlazorLibraryApp.Models;

namespace BlazorLibraryApp.Managers
{
    public class DashboardManager
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;

        public List<Book> Books { get; private set; } = new();
        public List<Author> Authors { get; private set; } = new();
        public List<Category> Categories { get; private set; } = new();

        public DashboardManager(
            IBookService bookService,
            IAuthorService authorService,
            ICategoryService categoryService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _categoryService = categoryService;
        }

        public async Task LoadDataAsync()
        {
            Books = await _bookService.GetAllBooksAsync();
            Authors = await _authorService.GetAllAuthorsAsync();
            Categories = await _categoryService.GetAllCategoriesAsync();
        }

        // Statistics
        public int TotalBooks => Books?.Count ?? 0;
        public int TotalAuthors => Authors?.Count ?? 0;
        public int TotalCategories => Categories?.Count ?? 0;
        public int BooksThisMonth => Books?.Count(b => b.CreatedAt.Month == DateTime.Now.Month && b.CreatedAt.Year == DateTime.Now.Year) ?? 0;

        // Books by Year
        public List<(int Year, int Count)> GetBooksByYear()
        {
            if (Books == null || !Books.Any())
                return new List<(int, int)>();

            return Books
                .GroupBy(b => b.Year)
                .OrderBy(g => g.Key)
                .Select(g => (g.Key, g.Count()))
                .ToList();
        }

        // Top Categories
        public List<(string Name, int Count, double Percentage)> GetTopCategories(int top = 5)
        {
            if (Books == null || !Books.Any())
                return new List<(string, int, double)>();

            var totalBooks = Books.Count;
            var categoryStats = Books
                .SelectMany(b => b.BookCategories)
                .Where(bc => bc.Category != null)
                .GroupBy(bc => bc.Category!.Name)
                .Select(g => (
                    Name: g.Key,
                    Count: g.Count(),
                    Percentage: Math.Round((double)g.Count() / totalBooks * 100, 1)
                ))
                .OrderByDescending(x => x.Count)
                .Take(top)
                .ToList();

            return categoryStats;
        }

        // Top Authors
        public List<(string Name, int BookCount)> GetTopAuthors(int top = 5)
        {
            if (Books == null || !Books.Any())
                return new List<(string, int)>();

            return Books
                .Where(b => b.Author != null)
                .GroupBy(b => b.Author!.Name)
                .Select(g => (Name: g.Key, BookCount: g.Count()))
                .OrderByDescending(x => x.BookCount)
                .Take(top)
                .ToList();
        }

        // Recent Books (Last 30 days trend)
        public List<(DateTime Date, int Count)> GetRecentBooksTrend(int days = 30)
        {
            if (Books == null || !Books.Any())
                return new List<(DateTime, int)>();

            var startDate = DateTime.Now.AddDays(-days);
            
            return Enumerable.Range(0, days + 1)
                .Select(i => startDate.AddDays(i))
                .Select(date => (
                    Date: date,
                    Count: Books.Count(b => b.CreatedAt.Date == date.Date)
                ))
                .ToList();
        }

        // Average books per author
        public double AverageBooksPerAuthor => Authors?.Any() == true ? (double)TotalBooks / TotalAuthors : 0;

        // Most recent book
        public Book? MostRecentBook => Books?.OrderByDescending(b => b.CreatedAt).FirstOrDefault();
    }
}

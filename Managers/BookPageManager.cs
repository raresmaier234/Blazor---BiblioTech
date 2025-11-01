using BlazorLibraryApp.Models;
using BlazorLibraryApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorLibraryApp.Managers
{
    public class BookPageManager
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;
        private readonly NavigationManager _navigation;

        public BookPageManager(
            IBookService bookService,
            IAuthorService authorService,
            ICategoryService categoryService,
            NavigationManager navigation)
        {
            _bookService = bookService;
            _authorService = authorService;
            _categoryService = categoryService;
            _navigation = navigation;
        }

        // State
        public List<Book>? Books { get; private set; }
        public List<Book>? FilteredBooks { get; private set; }
        public List<Author>? Authors { get; private set; }
        public List<Category>? Categories { get; private set; }
        public bool ShowAddForm { get; set; }
        public bool ShowEditForm { get; set; }
        public string SearchTerm { get; set; } = "";
        public string FilterAuthorId { get; set; } = "";
        public string FilterCategoryId { get; set; } = "";
        public Book CurrentBook { get; set; } = new Book { Title = "" };
        public HashSet<int> SelectedCategoryIds { get; set; } = new();
        public string ErrorMessage { get; set; } = "";
        public bool ShowCategoryError { get; set; }

        // Load Data
        public async Task LoadDataAsync()
        {
            try
            {
                Books = await _bookService.GetAllBooksAsync();
                Authors = await _authorService.GetAllAuthorsAsync();
                Categories = await _categoryService.GetAllCategoriesAsync();
                FilterBooks();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        // Filter Books
        public void FilterBooks()
        {
            if (Books == null)
            {
                FilteredBooks = new List<Book>();
                return;
            }

            FilteredBooks = Books.Where(book =>
                (string.IsNullOrEmpty(SearchTerm) ||
                 book.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                 (book.Author?.Name != null && book.Author.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrEmpty(FilterAuthorId) ||
                 book.AuthorId.ToString() == FilterAuthorId) &&
                (string.IsNullOrEmpty(FilterCategoryId) ||
                 book.BookCategories.Any(bc => bc.CategoryId.ToString() == FilterCategoryId))
            ).ToList();
        }

        // Apply Filters with Navigation
        public void ApplyFilters()
        {
            var queryParams = new Dictionary<string, string?>();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                queryParams["search"] = SearchTerm;
            }
            if (!string.IsNullOrEmpty(FilterAuthorId))
            {
                queryParams["authorId"] = FilterAuthorId;
            }
            if (!string.IsNullOrEmpty(FilterCategoryId))
            {
                queryParams["categoryId"] = FilterCategoryId;
            }

            var uri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("/books", queryParams);
            _navigation.NavigateTo(uri);
        }

        // Clear Filters
        public void ClearFilters()
        {
            SearchTerm = "";
            FilterAuthorId = "";
            FilterCategoryId = "";
            _navigation.NavigateTo("/books");
        }

        // Load Filters from Query String
        public void LoadFiltersFromQueryString()
        {
            var uri = _navigation.ToAbsoluteUri(_navigation.Uri);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            SearchTerm = query.TryGetValue("search", out var searchValue) ? searchValue.ToString() : "";
            FilterAuthorId = query.TryGetValue("authorId", out var authorValue) ? authorValue.ToString() : "";
            FilterCategoryId = query.TryGetValue("categoryId", out var categoryValue) ? categoryValue.ToString() : "";

            FilterBooks();
        }

        // Toggle Add Form
        public void ToggleAddForm()
        {
            ShowAddForm = !ShowAddForm;
            ShowEditForm = false;
            if (ShowAddForm)
            {
                CurrentBook = new Book { Title = "" };
                SelectedCategoryIds.Clear();
            }
        }

        // Show Edit Form
        public void ShowEditFormFor(Book book)
        {
            ShowEditForm = true;
            ShowAddForm = false;
            CurrentBook = new Book
            {
                Id = book.Id,
                Title = book.Title,
                Year = book.Year,
                AuthorId = book.AuthorId,
                Author = book.Author
            };
            SelectedCategoryIds = book.BookCategories?.Select(bc => bc.CategoryId).ToHashSet() ?? new HashSet<int>();
        }

        // Toggle Category
        public void ToggleCategory(int categoryId, bool isSelected)
        {
            if (isSelected)
            {
                SelectedCategoryIds.Add(categoryId);
            }
            else
            {
                SelectedCategoryIds.Remove(categoryId);
            }

            if (SelectedCategoryIds.Any())
            {
                ShowCategoryError = false;
            }
        }

        // Cancel Form
        public void CancelForm()
        {
            ShowAddForm = false;
            ShowEditForm = false;
            SelectedCategoryIds.Clear();
            ErrorMessage = "";
            ShowCategoryError = false;
        }

        // Save Book
        public async Task<bool> SaveBookAsync()
        {
            try
            {
                ErrorMessage = "";
                ShowCategoryError = false;

                if (CurrentBook.AuthorId == 0)
                {
                    ErrorMessage = "Te rugăm să selectezi un autor.";
                    return false;
                }

                if (!ShowEditForm && !SelectedCategoryIds.Any())
                {
                    ShowCategoryError = true;
                    ErrorMessage = "Te rugăm să selectezi cel puțin o categorie pentru carte.";
                    return false;
                }

                if (ShowEditForm)
                {
                    await _bookService.UpdateBookWithCategoriesAsync(CurrentBook.Id, CurrentBook, SelectedCategoryIds.ToList());
                }
                else
                {
                    await _bookService.CreateBookWithCategoriesAsync(CurrentBook, SelectedCategoryIds.ToList());
                }

                await LoadDataAsync();
                CancelForm();
                return true;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = "A apărut o eroare la salvarea cărții. Te rugăm să încerci din nou.";
                Console.WriteLine($"Error saving book: {ex.Message}");
                return false;
            }
        }

        // Delete Book
        public async Task<bool> DeleteBookAsync(int bookId)
        {
            try
            {
                await _bookService.DeleteBookAsync(bookId);
                await LoadDataAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
                return false;
            }
        }
    }
}

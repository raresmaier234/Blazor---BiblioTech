using BlazorLibraryApp.Models;
using BlazorLibraryApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorLibraryApp.Managers
{
    public class AuthorPageManager
    {
        private readonly IAuthorService _authorService;
        private readonly NavigationManager _navigation;

        public AuthorPageManager(IAuthorService authorService, NavigationManager navigation)
        {
            _authorService = authorService;
            _navigation = navigation;
        }

        // State
        public List<Author>? Authors { get; private set; }
        public List<Author>? FilteredAuthors { get; private set; }
        public bool ShowAddForm { get; set; }
        public bool ShowEditForm { get; set; }
        public string SearchTerm { get; set; } = "";
        public Author CurrentAuthor { get; set; } = new Author { Name = "", Email = "" };
        public string ErrorMessage { get; set; } = "";

        // Load Data
        public async Task LoadDataAsync()
        {
            try
            {
                Authors = await _authorService.GetAllAuthorsAsync();
                FilterAuthors();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading authors: {ex.Message}");
            }
        }

        // Filter Authors
        public void FilterAuthors()
        {
            if (Authors == null)
            {
                FilteredAuthors = new List<Author>();
                return;
            }

            FilteredAuthors = Authors.Where(author =>
                string.IsNullOrEmpty(SearchTerm) ||
                author.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (author.Email != null && author.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        // Apply Filters
        public void ApplyFilters()
        {
            var queryParams = new Dictionary<string, string?>();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                queryParams["search"] = SearchTerm;
            }

            var uri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("/authors", queryParams);
            _navigation.NavigateTo(uri);
        }

        // Clear Filters
        public void ClearFilters()
        {
            SearchTerm = "";
            _navigation.NavigateTo("/authors");
        }

        // Load Filters from Query String
        public void LoadFiltersFromQueryString()
        {
            var uri = _navigation.ToAbsoluteUri(_navigation.Uri);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            SearchTerm = query.TryGetValue("search", out var searchValue) ? searchValue.ToString() : "";

            FilterAuthors();
        }

        // Toggle Add Form
        public void ToggleAddForm()
        {
            ShowAddForm = !ShowAddForm;
            ShowEditForm = false;
            if (ShowAddForm)
            {
                CurrentAuthor = new Author { Name = "", Email = "" };
                ErrorMessage = "";
            }
        }

        // Show Edit Form
        public void ShowEditFormFor(Author author)
        {
            ShowEditForm = true;
            ShowAddForm = false;
            CurrentAuthor = new Author
            {
                Id = author.Id,
                Name = author.Name,
                Email = author.Email,
                Biography = author.Biography
            };
            ErrorMessage = "";
        }

        // Cancel Form
        public void CancelForm()
        {
            ShowAddForm = false;
            ShowEditForm = false;
            ErrorMessage = "";
        }

        // Save Author
        public async Task<bool> SaveAuthorAsync()
        {
            try
            {
                ErrorMessage = "";

                if (ShowEditForm)
                {
                    await _authorService.UpdateAuthorAsync(CurrentAuthor.Id, CurrentAuthor);
                }
                else
                {
                    await _authorService.CreateAuthorAsync(CurrentAuthor);
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
                ErrorMessage = "A apărut o eroare la salvarea autorului. Te rugăm să încerci din nou.";
                Console.WriteLine($"Error saving author: {ex.Message}");
                return false;
            }
        }

        // Delete Author
        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            try
            {
                await _authorService.DeleteAuthorAsync(authorId);
                await LoadDataAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting author: {ex.Message}");
                return false;
            }
        }
    }
}

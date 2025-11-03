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

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages => FilteredAuthors != null && FilteredAuthors.Any()
            ? (int)Math.Ceiling(FilteredAuthors.Count / (double)ItemsPerPage)
            : 1;
        public int TotalItems => FilteredAuthors?.Count ?? 0;
        public int StartItem => (CurrentPage - 1) * ItemsPerPage + 1;
        public int EndItem => Math.Min(CurrentPage * ItemsPerPage, TotalItems);

        public List<Author> PaginatedAuthors => FilteredAuthors?
            .Skip((CurrentPage - 1) * ItemsPerPage)
            .Take(ItemsPerPage)
            .ToList() ?? new List<Author>();

        public void SetItemsPerPage(int items)
        {
            ItemsPerPage = items;
            CurrentPage = 1;
            UpdateQueryString();
        }

        public void GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
                UpdateQueryString();
            }
        }

        public void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdateQueryString();
            }
        }

        public void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdateQueryString();
            }
        }

        public List<int> GetPageNumbers()
        {
            var pages = new List<int>();
            var start = Math.Max(1, CurrentPage - 2);
            var end = Math.Min(TotalPages, CurrentPage + 2);

            for (int i = start; i <= end; i++)
            {
                pages.Add(i);
            }

            return pages;
        }

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
        public void FilterAuthors(bool resetPage = false)
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
            
            // Reset to page 1 only when filters change
            if (resetPage)
            {
                CurrentPage = 1;
            }
        }

        // Apply Filters
        public void ApplyFilters()
        {
            var queryParams = new Dictionary<string, string?>();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                queryParams["search"] = SearchTerm;
            }
            
            queryParams["page"] = "1";
            queryParams["perPage"] = ItemsPerPage.ToString();

            var uri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("/authors", queryParams);
            _navigation.NavigateTo(uri);
            
            FilterAuthors(resetPage: true);
        }

        private void UpdateQueryString()
        {
            var queryParams = new Dictionary<string, string?>();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                queryParams["search"] = SearchTerm;
            }
            
            queryParams["page"] = CurrentPage.ToString();
            queryParams["perPage"] = ItemsPerPage.ToString();

            var uri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("/authors", queryParams);
            _navigation.NavigateTo(uri, forceLoad: false);
        }

        // Clear Filters
        public void ClearFilters()
        {
            SearchTerm = "";
            CurrentPage = 1;
            ItemsPerPage = 10;
            _navigation.NavigateTo("/authors");
        }

        // Load Filters from Query String
        public void LoadFiltersFromQueryString()
        {
            var uri = _navigation.ToAbsoluteUri(_navigation.Uri);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            SearchTerm = query.TryGetValue("search", out var searchValue) ? searchValue.ToString() : "";
            
            if (query.TryGetValue("page", out var pageValue) && int.TryParse(pageValue, out var page))
            {
                CurrentPage = page;
            }
            
            if (query.TryGetValue("perPage", out var perPageValue) && int.TryParse(perPageValue, out var perPage))
            {
                ItemsPerPage = perPage;
            }

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

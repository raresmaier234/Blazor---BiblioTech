using BlazorLibraryApp.Models;
using BlazorLibraryApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorLibraryApp.Managers
{
    public class CategoryPageManager
    {
        private readonly ICategoryService _categoryService;
        private readonly NavigationManager _navigation;

        public CategoryPageManager(ICategoryService categoryService, NavigationManager navigation)
        {
            _categoryService = categoryService;
            _navigation = navigation;
        }

        // State
        public List<Category>? Categories { get; private set; }
        public List<Category>? FilteredCategories { get; private set; }
        public bool ShowAddForm { get; set; }
        public bool ShowEditForm { get; set; }
        public string SearchTerm { get; set; } = "";
        public Category CurrentCategory { get; set; } = new Category { Name = "" };
        public string ErrorMessage { get; set; } = "";

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages => FilteredCategories != null && FilteredCategories.Any()
            ? (int)Math.Ceiling(FilteredCategories.Count / (double)ItemsPerPage)
            : 1;
        public int TotalItems => FilteredCategories?.Count ?? 0;
        public int StartItem => (CurrentPage - 1) * ItemsPerPage + 1;
        public int EndItem => Math.Min(CurrentPage * ItemsPerPage, TotalItems);

        public List<Category> PaginatedCategories => FilteredCategories?
            .Skip((CurrentPage - 1) * ItemsPerPage)
            .Take(ItemsPerPage)
            .ToList() ?? new List<Category>();

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
                Categories = await _categoryService.GetAllCategoriesAsync();
                FilterCategories();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
            }
        }

        // Filter Categories
        public void FilterCategories(bool resetPage = false)
        {
            if (Categories == null)
            {
                FilteredCategories = new List<Category>();
                return;
            }

            FilteredCategories = Categories.Where(category =>
                string.IsNullOrEmpty(SearchTerm) ||
                category.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (category.Description != null && category.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
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

            var uri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("/categories", queryParams);
            _navigation.NavigateTo(uri);
            
            FilterCategories(resetPage: true);
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

            var uri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("/categories", queryParams);
            _navigation.NavigateTo(uri, forceLoad: false);
        }

        // Clear Filters
        public void ClearFilters()
        {
            SearchTerm = "";
            CurrentPage = 1;
            ItemsPerPage = 10;
            _navigation.NavigateTo("/categories");
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

            FilterCategories();
        }

        // Toggle Add Form
        public void ToggleAddForm()
        {
            ShowAddForm = !ShowAddForm;
            ShowEditForm = false;
            if (ShowAddForm)
            {
                CurrentCategory = new Category { Name = "" };
                ErrorMessage = "";
            }
        }

        // Show Edit Form
        public void ShowEditFormFor(Category category)
        {
            ShowEditForm = true;
            ShowAddForm = false;
            CurrentCategory = new Category
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
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

        // Save Category
        public async Task<bool> SaveCategoryAsync()
        {
            try
            {
                ErrorMessage = "";

                if (ShowEditForm)
                {
                    await _categoryService.UpdateCategoryAsync(CurrentCategory.Id, CurrentCategory);
                }
                else
                {
                    await _categoryService.CreateCategoryAsync(CurrentCategory);
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
                ErrorMessage = "A apărut o eroare la salvarea categoriei. Te rugăm să încerci din nou.";
                Console.WriteLine($"Error saving category: {ex.Message}");
                return false;
            }
        }

        // Delete Category
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(categoryId);
                await LoadDataAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
                return false;
            }
        }
    }
}

using BlazorLibraryApp.Models;
using BlazorLibraryApp.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorLibraryApp.Managers
{
    public class BookDetailsPageManager
    {
        private readonly IBookService _bookService;
        private readonly NavigationManager _navigation;

        public Book? CurrentBook { get; private set; }

        public BookDetailsPageManager(IBookService bookService, NavigationManager navigation)
        {
            _bookService = bookService;
            _navigation = navigation;
        }

        public async Task LoadBookAsync(int bookId)
        {
            try
            {
                CurrentBook = await _bookService.GetBookByIdAsync(bookId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading book: {ex.Message}");
                CurrentBook = null;
            }
        }

        public void NavigateToBooks()
        {
            _navigation.NavigateTo("/books");
        }

        public void NavigateToEdit(int bookId)
        {
            _navigation.NavigateTo($"/books?edit={bookId}");
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            try
            {
                await _bookService.DeleteBookAsync(bookId);
                NavigateToBooks();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
                return false;
            }
        }

        public string GetDaysAgo()
        {
            if (CurrentBook == null) return "N/A";

            var romanianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Bucharest");
            var currentRomanianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, romanianTimeZone);
            var timeSpan = currentRomanianTime - CurrentBook.CreatedAt;

            if (timeSpan.TotalDays < 1)
            {
                if (timeSpan.TotalHours < 1)
                {
                    return $"{(int)timeSpan.TotalMinutes} min";
                }
                return $"{(int)timeSpan.TotalHours}h";
            }
            else if (timeSpan.TotalDays < 30)
            {
                return $"{(int)timeSpan.TotalDays} zile";
            }
            else if (timeSpan.TotalDays < 365)
            {
                return $"{(int)(timeSpan.TotalDays / 30)} luni";
            }
            else
            {
                return $"{(int)(timeSpan.TotalDays / 365)} ani";
            }
        }
    }
}

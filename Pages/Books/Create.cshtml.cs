using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LMS.Razor.Data;
using Microsoft.Data.SqlClient;

namespace LMS.Razor.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly Database _db;
        [BindProperty] public string Title { get; set; } = "";
        [BindProperty] public string Author { get; set; } = "";
        [BindProperty] public int AvailableCopies { get; set; } = 1;

        public CreateModel(Database db) => _db = db;

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Author))
            {
                ModelState.AddModelError("", "Title and Author are required.");
                return Page();
            }

            _db.Execute(
                "INSERT INTO Books (Title, Author, AvailableCopies) VALUES (@t, @a, @c)",
                new SqlParameter("@t", Title),
                new SqlParameter("@a", Author),
                new SqlParameter("@c", AvailableCopies)
            );
            return RedirectToPage("Index");
        }
    }
}
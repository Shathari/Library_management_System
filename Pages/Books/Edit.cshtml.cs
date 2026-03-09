using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LMS.Razor.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LMS.Razor.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly Database _db;
        [BindProperty] public int BookId { get; set; }
        [BindProperty] public string Title { get; set; } = "";
        [BindProperty] public string Author { get; set; } = "";
        [BindProperty] public int AvailableCopies { get; set; }

        public EditModel(Database db) => _db = db;

        public IActionResult OnGet(int id)
        {
            var dt = _db.Query("SELECT * FROM Books WHERE BookId=@id", new SqlParameter("@id", id));
            if (dt.Rows.Count == 0) return RedirectToPage("Index");

            var r = dt.Rows[0];
            BookId = (int)r["BookId"];
            Title  = r["Title"].ToString()!;
            Author = r["Author"].ToString()!;
            AvailableCopies = Convert.ToInt32(r["AvailableCopies"]);
            return Page();
        }

        public IActionResult OnPost()
        {
            if (BookId <= 0) return RedirectToPage("Index");
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Author))
            {
                ModelState.AddModelError("", "Title and Author are required.");
                return Page();
            }

            _db.Execute(
                "UPDATE Books SET Title=@t, Author=@a, Copies=@c WHERE BookId=@id",
                new SqlParameter("@t", Title),
                new SqlParameter("@a", Author),
                new SqlParameter("@c", AvailableCopies),
                new SqlParameter("@id", BookId)
            );
            return RedirectToPage("Index");
        }
    }
}
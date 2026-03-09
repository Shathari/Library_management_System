using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LMS.Razor.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LMS.Razor.Pages.Books
{
    public class DeleteModel : PageModel
    {
        private readonly Database _db;
        public DataRow? Book { get; set; }

        public DeleteModel(Database db) => _db = db;

        public IActionResult OnGet(int id)
        {
            var dt = _db.Query("SELECT TOP 1 * FROM dbo.Books WHERE BookId=@id",
                               new SqlParameter("@id", id));
            if (dt.Rows.Count == 0) return RedirectToPage("Index");
            Book = dt.Rows[0];
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            _db.Execute("DELETE FROM dbo.Books WHERE BookId=@id",
                        new SqlParameter("@id", id));
            return RedirectToPage("Index");
        }
    }
}
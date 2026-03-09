using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LMS.Razor.Data;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace LMS.Razor.Pages.Members
{
    public class CreateModel : PageModel
    {
        private readonly Database _db;

        [BindProperty, Required, StringLength(100)]
        public string FullName { get; set; } = "";

        [BindProperty, EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        [BindProperty, Phone, StringLength(20)]
        public string? Phone { get; set; }

        public CreateModel(Database db) => _db = db;

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // Explicitly set CreatedAt at insert time
            _db.Execute(@"
                INSERT INTO Members (FullName, Email, Phone, CreatedAt)
                VALUES (@n, @e, @p, GETDATE());",
                new SqlParameter("@n", (object)FullName ?? DBNull.Value),
                new SqlParameter("@e", (object?)Email ?? DBNull.Value),
                new SqlParameter("@p", (object?)Phone ?? DBNull.Value)
            );

            return RedirectToPage("Index");
        }
    }
}
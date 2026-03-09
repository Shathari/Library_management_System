using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LMS.Razor.Data;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LMS.Razor.Pages.Members
{
    public class EditModel : PageModel
    {
        private readonly Database _db;

        [BindProperty]
        public int MemberId { get; set; }

        [BindProperty, Required, StringLength(100)]
        public string FullName { get; set; } = "";

        [BindProperty, EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        [BindProperty, Phone, StringLength(20)]
        public string? Phone { get; set; }

        [BindProperty]
        public DateTime? CreatedAt { get; set; } // display-only

        public EditModel(Database db) => _db = db;

        public IActionResult OnGet(int id)
        {
            var dt = _db.Query("SELECT * FROM Members WHERE MemberId=@id",
                new SqlParameter("@id", id));

            if (dt.Rows.Count == 0)
                return RedirectToPage("Index");

            var r = dt.Rows[0];
            MemberId = (int)r["MemberId"];
            FullName = r["FullName"].ToString() ?? "";
            Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString();
            Phone = r["Phone"] == DBNull.Value ? null : r["Phone"].ToString();
            CreatedAt = r["CreatedAt"] == DBNull.Value ? (DateTime?)null : (DateTime)r["CreatedAt"];

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _db.Execute(@"
                UPDATE Members
                SET FullName=@n, Email=@e, Phone=@p
                WHERE MemberId=@id;",
                new SqlParameter("@n", (object)FullName ?? DBNull.Value),
                new SqlParameter("@e", (object?)Email ?? DBNull.Value),
                new SqlParameter("@p", (object?)Phone ?? DBNull.Value),
                new SqlParameter("@id", MemberId)
            );

            return RedirectToPage("Index");
        }
    }
}
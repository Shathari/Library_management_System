using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LMS.Razor.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LMS.Razor.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly Database _db;
        public DataTable BooksTable { get; private set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? q { get; set; }

        public IndexModel(Database db) => _db = db;

        public void OnGet()
        {
            var term = q?.Trim() ?? string.Empty;
            var pattern = $"%{term}%";

            var sql = @"
                SELECT BookId, Title, Author, AvailableCopies, TotalCopies
                FROM dbo.Books
                WHERE (@q = '' 
                       OR Title  LIKE @pattern
                       OR Author LIKE @pattern
                       OR Isbn   LIKE @pattern)
                ORDER BY Title;";

            BooksTable = _db.Query(sql,
                new SqlParameter("@q", term),
                new SqlParameter("@pattern", pattern));
        }
    }
}
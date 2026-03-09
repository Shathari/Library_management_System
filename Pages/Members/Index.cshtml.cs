using Microsoft.AspNetCore.Mvc.RazorPages;
using LMS.Razor.Data;
using System.Data;

namespace LMS.Razor.Pages.Members
{
    public class IndexModel : PageModel
    {
        private readonly Database _db;
        public DataTable MembersTable { get; set; } = new();

        public IndexModel(Database db) => _db = db;

        public void OnGet()
        {
            MembersTable = _db.Query(@"
                SELECT MemberId, FullName, Email, Phone, CreatedAt
                FROM Members
                ORDER BY CreatedAt DESC, FullName ASC;");
        }
    }
}
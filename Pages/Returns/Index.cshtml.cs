using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LMS.Razor.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LMS.Razor.Pages.Returns
{
    public class IndexModel : PageModel
    {
        private readonly Database _db;
        [BindProperty] public int LoanId { get; set; }
        public string? Message { get; set; }
        public DataTable? Result { get; set; }

        public IndexModel(Database db) => _db = db;

        public void OnGet() { }

        public void OnPost()
        {
            if (LoanId <= 0)
            {
                Message = "Enter a valid LoanId.";
                return;
            }

            try
            {
                Result = _db.ExecProc("dbo.sp_ReturnBook",
                                      new SqlParameter("@LoanId", LoanId));
                Message = "Return processed successfully.";
            }
            catch (Exception ex)
            {
                Message = "Error: " + ex.Message;
            }
        }
    }
}
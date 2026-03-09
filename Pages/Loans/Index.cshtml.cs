using LMS.Razor.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LMS.Razor.Pages.Loans
{
    public class IndexModel : PageModel
    {
        private readonly LoansRepository _repo;
        public IndexModel(LoansRepository repo) => _repo = repo;

        public List<LoanRecord> Items { get; private set; } = new();

        public async Task OnGetAsync()
        {
            Items = await _repo.GetAllAsync();
        }
    }
}
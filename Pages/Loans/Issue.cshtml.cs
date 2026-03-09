using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // for SelectList
using LMS.Razor.Data;

namespace LMS.Razor.Pages.Loans
{
    public class IssueModel : PageModel
    {
        private readonly LoansRepository _repo;

        public IssueModel(LoansRepository repo) => _repo = repo;

        // ✅ Matches your view's Model.Input.*
        public class InputModel
        {
            public int MemberId { get; set; }
            public int BookId { get; set; }

            // Internal property we use for due-date calculation
            public int Days { get; set; } = 7;

            // ✅ Alias so your view's Input.LoanDays works without changes
            public int LoanDays
            {
                get => Days;
                set => Days = value;
            }
        }

        // Bound model for <input asp-for="Input.*">
        [BindProperty]
        public InputModel Input { get; set; } = new();

        // ✅ Matches your view's MemberOptions / BookOptions
        public SelectList? MemberOptions { get; set; }
        public SelectList? BookOptions { get; set; }

        // Grid (recent loans list)
        public List<LoanRecord> Loans { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadLookupsAndGridAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAndGridAsync();
                return Page();
            }

            if (Input.MemberId <= 0 || Input.BookId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please select both member and book.");
                await LoadLookupsAndGridAsync();
                return Page();
            }

            // ✅ Calculate due date from Input.Days (which your view binds via Input.LoanDays)
            if (Input.Days <= 0) Input.Days = 7;
            var dueDate = DateTime.Today.AddDays(Input.Days);

            try
            {
                await _repo.IssueAsync(Input.MemberId, Input.BookId, dueDate);
                TempData["Message"] = "Book issued successfully.";
                return RedirectToPage(); // PRG pattern
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadLookupsAndGridAsync();
                return Page();
            }
        }

        private async Task LoadLookupsAndGridAsync()
        {
            var members = await _repo.GetMembersAsync();
            var books   = await _repo.GetAvailableBooksAsync();
            Loans       = await _repo.GetAllAsync();

            // ✅ Use string property names (or nameof) for SelectList
            MemberOptions = new SelectList(members, nameof(MemberDto.MemberId), nameof(MemberDto.FullName));
            BookOptions   = new SelectList(books,   nameof(BookDto.BookId),    nameof(BookDto.Title));
        }
    }
}
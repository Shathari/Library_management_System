namespace LMS.Razor.Data
{
    public class LoansRecord
    {
        public int LoanId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = "";
        public int MemberId { get; set; }
        public string MemberName { get; set; } = "";
        public DateTime IssuedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }

    public class MemberDto
    {
        public int MemberId { get; set; }
        public string FullName { get; set; } = "";
    }

    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
    }
}
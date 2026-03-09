namespace LMS.Razor.Data
{
    public class LoanRecord
    {
        public int LoanId { get; set; }
        public int MemberId { get; set; }
        public int BookId { get; set; }
        public string MemberName { get; set; } = "";
        public string BookTitle { get; set; } = "";
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal? FineAmount { get; set; }
    }

    public class SimpleOption
    {
        public int Id { get; set; }
        public string Label { get; set; } = "";
    }
}
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LMS.Razor.Data
{
    /// <summary>
    /// Repository for Loans-related operations.
    /// Relies on Database helper (Query/Execute/ExecProc).
    /// Assumes DTOs LoanRecord, MemberDto, BookDto exist in LMS.Razor.Data namespace.
    /// </summary>
    public class LoansRepository
    {
        private readonly Database _db;

        public LoansRepository(Database db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns all loans joined with member & book info.
        /// Uses SQL aliases to map your DB columns (IssuedOn, DueOn, ReturnedOn)
        /// to DTO properties (IssuedAt, DueDate, ReturnedAt).
        /// </summary>
        public Task<List<LoanRecord>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    l.LoanId,
                    l.BookId,
                    b.Title       AS BookTitle,
                    l.MemberId,
                    m.FullName    AS MemberName,
                    l.IssuedOn    AS IssuedAt,   -- alias to DTO
                    l.DueOn       AS DueDate,    -- alias to DTO
                    l.ReturnedOn  AS ReturnedAt  -- alias to DTO
                FROM dbo.Loans AS l
                INNER JOIN dbo.Books   AS b ON b.BookId   = l.BookId
                INNER JOIN dbo.Members AS m ON m.MemberId = l.MemberId
                ORDER BY l.IssuedOn DESC;";

            var dt = _db.Query(sql);

            var list = new List<LoanRecord>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new LoanRecord
                {
                    LoanId     = Convert.ToInt32(r["LoanId"]),
                    BookId     = Convert.ToInt32(r["BookId"]),
                    BookTitle  = Convert.ToString(r["BookTitle"]) ?? "",
                    MemberId   = Convert.ToInt32(r["MemberId"]),
                    MemberName = Convert.ToString(r["MemberName"]) ?? "",
                    IssueDate   = Convert.ToDateTime(r["IssuedAt"]),
                    DueDate    = Convert.ToDateTime(r["DueDate"]),
                    ReturnDate = r["ReturnedAt"] == DBNull.Value
                                 ? (DateTime?)null
                                 : Convert.ToDateTime(r["ReturnedAt"])
                });
            }

            return Task.FromResult(list);
        }

        /// <summary>
        /// Members for dropdown (MemberId + FullName).
        /// </summary>
        public Task<List<MemberDto>> GetMembersAsync()
        {
            var dt = _db.Query(@"
                SELECT MemberId, FullName
                FROM dbo.Members
                ORDER BY FullName;");

            var list = new List<MemberDto>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new MemberDto
                {
                    MemberId = Convert.ToInt32(r["MemberId"]),
                    FullName = Convert.ToString(r["FullName"]) ?? ""
                });
            }

            return Task.FromResult(list);
        }

        /// <summary>
        /// Books with AvailableCopies > 0 for issuing.
        /// </summary>
        public Task<List<BookDto>> GetAvailableBooksAsync()
        {
            var dt = _db.Query(@"
                SELECT BookId, Title
                FROM dbo.Books
                WHERE AvailableCopies > 0
                ORDER BY Title;");

            var list = new List<BookDto>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new BookDto
                {
                    BookId = Convert.ToInt32(r["BookId"]),
                    Title  = Convert.ToString(r["Title"]) ?? ""
                });
            }

            return Task.FromResult(list);
        }

        /// <summary>
        /// Issues a book to a member:
        /// 1) Decrement AvailableCopies atomically (fail if none available)
        /// 2) Insert loan row with IssuedOn (now) and DueOn (parameter)
        /// </summary>
        public Task IssueAsync(int memberId, int bookId, DateTime dueDate)
        {
            var now = DateTime.UtcNow; // Use DateTime.Now if you prefer local server time

            var sql = @"
BEGIN TRAN;

UPDATE b
SET b.AvailableCopies = b.AvailableCopies - 1
FROM dbo.Books AS b
WHERE b.BookId = @BookId
  AND b.AvailableCopies > 0;

IF @@ROWCOUNT = 0
BEGIN
    ROLLBACK TRAN;
    RAISERROR('No copies available for this book.', 16, 1);
    RETURN;
END;

INSERT INTO dbo.Loans (BookId, MemberId, IssuedOn, DueOn)
VALUES (@BookId, @MemberId, @IssuedOn, @DueOn);

COMMIT TRAN;";

            _db.Execute(sql,
                new SqlParameter("@BookId",  bookId),
                new SqlParameter("@MemberId", memberId),
                new SqlParameter("@IssuedOn", now),
                new SqlParameter("@DueOn",    dueDate));

            return Task.CompletedTask;
        }

        /// <summary>
        /// (Optional) Active (not returned) loans as DataTable for quick diagnostics/views.
        /// </summary>
        public DataTable ListActive()
        {
            return _db.Query(@"
                SELECT l.LoanId, l.BookId, l.MemberId, l.IssuedOn, l.DueOn
                FROM dbo.Loans AS l
                WHERE l.ReturnedOn IS NULL
                ORDER BY l.IssuedOn DESC;");
        }

        /// <summary>
        /// Calls stored procedure to process a return (should set ReturnedOn, bump AvailableCopies).
        /// Ensure your dbo.sp_ReturnBook updates ReturnedOn (not ReturnedAt).
        /// </summary>
        public DataTable ReturnBook(int loanId)
        {
            return _db.ExecProc("dbo.sp_ReturnBook", new SqlParameter("@LoanId", loanId));
        }
    }
}
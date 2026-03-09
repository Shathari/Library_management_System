using System.Data;
using Microsoft.Data.SqlClient;

namespace LMS.Razor.Data
{
    public class BooksRepository
    {
        private readonly Database _db;

        public BooksRepository(Database db) // inject Database, not string
        {
            _db = db;
        }

        public DataTable List(string? q)
        {
            var term = q?.Trim() ?? "";
            var pattern = $"%{term}%";
            var sql = @"
                SELECT BookId, Title, Author, AvailableCopies, TotalCopies
                FROM dbo.Books
                WHERE (@q = '' OR Title LIKE @p OR Author LIKE @p OR Isbn LIKE @p)
                ORDER BY Title;";

            return _db.Query(sql,
                new SqlParameter("@q", term),
                new SqlParameter("@p", pattern));
        }

        public DataRow? Get(int id)
        {
            var dt = _db.Query("SELECT TOP 1 * FROM dbo.Books WHERE BookId=@id",
                               new SqlParameter("@id", id));
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public int Delete(int id)
        {
            return _db.Execute("DELETE FROM dbo.Books WHERE BookId=@id",
                               new SqlParameter("@id", id));
        }
    }
}
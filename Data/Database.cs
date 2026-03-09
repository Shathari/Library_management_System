using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LMS.Razor.Data
{
    public class Database
    {
        private readonly string _connStr;

        public Database(IConfiguration config)
        {
            _connStr = config.GetConnectionString("LmsDb")
                ?? throw new ArgumentNullException(nameof(config), "Connection string 'LmsDb' is missing.");
        }

        // --- TEXT SQL (SELECT)
        public DataTable Query(string sql, Dictionary<string, object>? parameters = null)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);
            AddParameters(cmd, parameters);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public DataTable Query(string sql, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);
            if (parameters is { Length: > 0 }) cmd.Parameters.AddRange(parameters);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        // --- TEXT SQL (NonQuery)
        public int Execute(string sql, Dictionary<string, object>? parameters = null)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);
            AddParameters(cmd, parameters);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int Execute(string sql, params SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);
            if (parameters is { Length: > 0 }) cmd.Parameters.AddRange(parameters);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // --- STORED PROCEDURES
        public DataTable ExecProc(string procName, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(procName, conn) { CommandType = CommandType.StoredProcedure };
            if (parameters is { Length: > 0 }) cmd.Parameters.AddRange(parameters);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public int ExecProcNonQuery(string procName, params SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(procName, conn) { CommandType = CommandType.StoredProcedure };
            if (parameters is { Length: > 0 }) cmd.Parameters.AddRange(parameters);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // --- Helper
        private static void AddParameters(SqlCommand cmd, Dictionary<string, object>? parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            foreach (var kvp in parameters)
            {
                var name = kvp.Key.StartsWith("@") ? kvp.Key : "@" + kvp.Key;
                var value = kvp.Value ?? DBNull.Value;

                if (value is string s)
                {
                    cmd.Parameters.Add(new SqlParameter(name, SqlDbType.NVarChar, 4000) { Value = s });
                }
                else
                {
                    cmd.Parameters.AddWithValue(name, value);
                }
            }
        }
    }
}
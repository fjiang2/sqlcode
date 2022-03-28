
using System.Data.SQLite;
using Sys.Data.Entity;

namespace Sys.Data.SQLite
{
    public class DbQuery : DataQuery
    {
        public DbQuery(string connectionString)
            : this(new SQLiteConnectionStringBuilder(connectionString))
        {
        }

        public DbQuery(SQLiteConnectionStringBuilder connectionString)
            : this(new SQLiteAgent(connectionString))
        {
        }

        public DbQuery(SQLiteAgent agent)
            : base(agent)
        {
        }

    }
}

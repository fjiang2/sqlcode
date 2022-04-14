using System.Data.Common;

namespace Sys.Data.SqlRedis
{
    public class SqlRedisConnectionStringBuilder : DbConnectionStringBuilder
    {

        public SqlRedisConnectionStringBuilder(string connectionString)
        {
            base.ConnectionString = connectionString;
        }

        public virtual string Provider
        {
            get => (string)this["Provider"];
            set => this["Provider"] = value;
        }

        public virtual string InitialCatalog
        {
            get => (string)this["Initial Catalog"];
            set => this["Initial Catalog"] = value;
        }


        public string DataSource
        {
            get => (string)this["Data Source"];
            set => this["Data Source"] = value;

        }

        public string UserId
        {
            get
            {
                if (this.ContainsKey("User Id"))
                    return (string)this["User Id"];
                else
                    return null;
            }
            set { this["User Id"] = value; }
        }

        public string Password
        {
            get
            {
                if (this.ContainsKey("Password"))
                    return (string)this["Password"];
                else
                    return null;
            }
            set { this["Password"] = value; }
        }
    }
}

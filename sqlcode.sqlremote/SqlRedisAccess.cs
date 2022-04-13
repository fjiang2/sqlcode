using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Sys.Data.SqlRemote;

namespace Sys.Data.SqlRedis
{

    public class SqlRedisAccess : SqlRemoteAccess
    {

        public SqlRedisAccess(string connectionString, SqlUnit unit)
            : base(new SqlRedisClient(connectionString), unit)
        {
        }
    }
}

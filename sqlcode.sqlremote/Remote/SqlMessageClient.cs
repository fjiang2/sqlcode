using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    class SqlMessageClient
    {
        private readonly SqlRequestMessage request;
        public readonly ISqlMessageClient client;

        public SqlMessageClient(ISqlMessageClient client, SqlRequestMessage request)
        {
            this.client = client;
            this.request = request;
        }

        public int ExecuteNonQuery()
        {
            SqlResultMessage result = Request();
            return result.Count;
        }

        public void ExecuteTransaction()
        {
            Request();
        }

        public object LoadScalar()
        {
            SqlResultMessage result = Request();
            return result.Scalar;
        }

        public int LoadDataSet(DataSet dataSet)
        {
            SqlResultMessage result = Request();
            result.FillDataSet(dataSet);
            return result.Count;
        }

        public int LoadDataTable(DataTable dataTable)
        {
            SqlResultMessage result = Request();
            result.FillDataTable(dataTable);
            return result.Count;
        }

        private SqlResultMessage Request()
        {
            SqlResultMessage result = client.RequesteAsync(request).Result;
            if (result.Error != null)
                throw new Exception(result.Error);

            return result;
        }
    }
}

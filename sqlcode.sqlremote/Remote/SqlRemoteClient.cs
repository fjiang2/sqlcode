using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    class SqlRemoteClient
    {
        private SqlRequest request;
        public string ConnectionString { get; }


        public SqlRemoteClient(string connectionString, SqlRequest request)
        {
            this.ConnectionString = connectionString;
            this.request = request;
        }

        public int ExecuteNonQuery()
        {
            SqlResult result = RequesteAsync().Result;
            return result.Count;
        }

        public void ExecuteTransaction()
        {
            RequesteAsync().Wait();
        }

        public object LoadScalar()
        {
            SqlResult result = RequesteAsync().Result;
            return result.Scalar;
        }

        public int LoadDataSet(DataSet dataSet)
        {
            SqlResult result = RequesteAsync().Result;
            result.FillDataSet(dataSet);
            return result.Count;
        }

        public int LoadDataTable(DataTable dataTable)
        {
            SqlResult result = RequesteAsync().Result;
            result.FillDataTable(dataTable);
            return result.Count;
        }

        public Task<SqlResult> RequesteAsync()
        {
            SqlResult result = new SqlResult();
            if(result.Error != null)
                throw new Exception(result.Error);


            var connection = new SqlRemoteConnectionStringBuilder(ConnectionString);
            switch(connection.Provider)
            {

                case "Redis":
                    break;

                case "Kafka":
                    break;

                case "http":
                    break;
            }

            return new Task<SqlResult>(() => result);
        }
    }
}

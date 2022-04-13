using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sys.Data.SqlRemote
{

    public class SqlRemoteAccess : DbAccess, IDbAccess
    {
        private readonly SqlRequestMessage request;

        private readonly string[] statements;
        private readonly IParameterFacet facet;

        private readonly SqlMessageClient client;

        public SqlRemoteAccess(ISqlMessageClient client, SqlUnit unit)
        {
            this.statements = unit.Statements;
            object args = unit.Arguments;
            string sql = unit.Statement;

            this.request = new SqlRequestMessage(sql)
            {
                CommandType = unit.CommandType,
            };

            this.client = new SqlMessageClient(client, request);

            if (args == null)
                return;

            this.facet = ParameterFacet.Create(args);

            List<IDataParameter> parameters = this.facet.CreateParameters();

            foreach (IDataParameter parameter in parameters)
            {
                object value = parameter.Value ?? DBNull.Value;
                SqlParameterMessage _parameter = new SqlParameterMessage
                {
                    ParameterName = parameter.ParameterName,
                    Value = value,
                    Direction = parameter.Direction
                };
                request.Parameters.Add(_parameter);
            }
        }


        public override int FillDataSet(DataSet dataSet)
        {
            request.Function = nameof(FillDataSet);

            return client.LoadDataSet(dataSet);
        }

        public override int ReadDataSet(DataSet dataSet)
        {
            request.Function = nameof(ReadDataSet);

            return client.LoadDataSet(dataSet);
        }

        public override int FillDataTable(DataTable dataTable, int startRecord, int maxRecords)
        {
            request.Function = nameof(FillDataTable);
            request.StartRecord = startRecord;
            request.MaxRecords = maxRecords;

            return client.LoadDataTable(dataTable);
        }

        public override int ReadDataTable(DataTable dataTable, int startRecord, int maxRecords)
        {
            request.Function = nameof(ReadDataTable);
            request.StartRecord = startRecord;
            request.MaxRecords = maxRecords;

            return client.LoadDataTable(dataTable);
        }

        public override int ExecuteNonQuery()
        {
            request.Function = nameof(ExecuteNonQuery);
            return client.ExecuteNonQuery();
        }

        public override object ExecuteScalar()
        {
            request.Function = nameof(ExecuteScalar);
            return client.LoadScalar();
        }

        public override void ExecuteTransaction()
        {
            if (statements.Length == 0)
                return;
            
            request.Function = nameof(ExecuteTransaction);

            client.ExecuteTransaction();
        }
    }
}

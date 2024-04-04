using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Sys.Data.Text;
using sqlcode.dynamodb.ado;

namespace Sys.Data.DynamoDb
{
    /// <summary>
    /// Access DynamoDB Service
    /// </summary>
    public class DynamoDbAccess : DbAccess, IDbAccess
    {
        private readonly DynamoDbCommand command;
        private readonly DynamoDbConnection connection;

        private readonly string[] statements;
        private readonly IParameterFacet? facet;

        public DynamoDbAccess(string connectionString, SqlUnit unit)
        {
            this.statements = unit.Statements;
            object args = unit.Arguments;
            string sql = unit.Statement;

            this.connection = new DynamoDbConnection(new DynamoDbConnectionStringBuilder(connectionString));
            this.command = new DynamoDbCommand(sql, connection)
            {
                CommandType = unit.CommandType,
                Connection = connection,
            };

            if (args == null)
                return;

            this.facet = ParameterFacet.Create(args);

            List<IDataParameter> parameters = this.facet.CreateParameters();
            foreach (IDataParameter parameter in parameters)
            {
                object value = parameter.Value ?? DBNull.Value;
                Parameter _parameter = NewParameter("@" + parameter.ParameterName, value, parameter.Direction);
                command.Parameters.Add(_parameter);
            }
        }

        private static Parameter NewParameter(string parameterName, object value, ParameterDirection direction)
        {
            Parameter param = new Parameter(parameterName, value)
            {
                Direction = direction,
            };

            return param;
        }


        public override int FillDataSet(DataSet dataSet)
        {
            try
            {
                connection.Open();
                DynamoDbDataAdapter adapter = new DynamoDbDataAdapter(command);
                return adapter.Fill(dataSet);
            }
            finally
            {
                connection.Close();
            }
        }

        public override int FillDataTable(DataTable dataTable, int startRecord, int maxRecords)
        {
            try
            {
                connection.Open();
                DynamoDbDataAdapter adapter = new DynamoDbDataAdapter(command);
                return adapter.Fill(startRecord, maxRecords, dataTable);
            }
            finally
            {
                connection.Close();
            }
        }

        public override int ReadDataSet(DataSet dataSet)
        {
            try
            {
                connection.Open();
                var reader = command.ExecuteReader();
                return new DbReader(reader).ReadDataSet(dataSet);
            }
            finally
            {
                connection.Close();
            }
        }

        public override int ReadDataTable(DataTable dataTable, int startRecord, int maxRecords)
        {
            try
            {
                connection.Open();
                var reader = command.ExecuteReader();
                return new DbReader(reader)
                {
                    StartRecord = startRecord,
                    MaxRecords = maxRecords,
                }.ReadTable(dataTable);
            }
            finally
            {
                connection.Close();
            }
        }


        public override int ExecuteNonQuery()
        {
            try
            {
                connection.Open();
                int count = command.ExecuteNonQuery();
                facet?.UpdateResult(command.Parameters.Cast<IDataParameter>());
                return count;
            }
            finally
            {
                connection.Close();
            }
        }


        public override object ExecuteScalar()
        {
            try
            {
                connection.Open();
                return command.ExecuteScalar();
            }
            finally
            {
                connection.Close();
            }
        }


        public override void ExecuteTransaction()
        {
            if (statements.Length == 0)
                return;

            try
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (string line in statements)
                        {
                            command.Transaction = transaction;
                            command.CommandText = line;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            finally
            {
                connection.Close();
            }

        }
    }
}

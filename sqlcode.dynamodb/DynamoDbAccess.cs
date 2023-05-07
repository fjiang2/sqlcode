using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sys.Data.DynamoDb
{
	/// <summary>
	/// Access SQL Server
	/// </summary>
	public class DynamoDbAccess : DbAccess, IDbAccess
	{
		private readonly SqlCommand command;
		private readonly SqlConnection connection;

		private readonly string[] statements;
		private readonly IParameterFacet facet;

		public DynamoDbAccess(string connectionString, SqlUnit unit)
		{
			this.statements = unit.Statements;
			object args = unit.Arguments;
			string sql = unit.Statement;

			this.connection = new SqlConnection(connectionString);
			this.command = new SqlCommand(sql)
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
				SqlParameter _parameter = NewParameter("@" + parameter.ParameterName, value, parameter.Direction);
				command.Parameters.Add(_parameter);
			}
		}

		private static SqlParameter NewParameter(string parameterName, object value, ParameterDirection direction)
		{
			SqlDbType dbType = SqlDbType.NVarChar;
			if (value is int)
				dbType = SqlDbType.Int;
			else if (value is short)
				dbType = SqlDbType.SmallInt;
			else if (value is long)
				dbType = SqlDbType.BigInt;
			else if (value is byte)
				dbType = SqlDbType.TinyInt;
			else if (value is DateTime)
				dbType = SqlDbType.DateTime;
			else if (value is double)
				dbType = SqlDbType.Float;
			else if (value is float)
				dbType = SqlDbType.Float;
			else if (value is decimal)
				dbType = SqlDbType.Decimal;
			else if (value is bool)
				dbType = SqlDbType.Bit;
			else if (value is string && ((string)value).Length > 4000)
				dbType = SqlDbType.NText;
			else if (value is string)
				dbType = SqlDbType.NVarChar;
			else if (value is byte[])
				dbType = SqlDbType.Binary;
			else if (value is Guid)
				dbType = SqlDbType.UniqueIdentifier;

			SqlParameter param = new SqlParameter(parameterName, dbType)
			{
				Value = value,
				Direction = direction,
			};

			return param;
		}


		public override int FillDataSet(DataSet dataSet)
		{
			try
			{
				connection.Open();
				SqlDataAdapter adapter = new SqlDataAdapter(command);
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
				SqlDataAdapter adapter = new SqlDataAdapter(command);
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

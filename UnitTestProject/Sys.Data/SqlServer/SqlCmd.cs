using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace Sys.Data
{

	public class SqlCmd : BaseDbCmd, IDbCmd
	{
		private readonly SqlCommand command;
		private readonly SqlConnection connection;
		private readonly IParameterFactory parameters;

		public SqlCmd(SqlConnectionStringBuilder connectionString, SqlUnit unit)
			: this(connectionString, unit.Statement, unit.Arguments)
		{
		}

		public SqlCmd(SqlConnectionStringBuilder connectionString, string sql, object args)
		{
			this.connection = new SqlConnection(connectionString.ConnectionString);

			this.command = new SqlCommand(sql);
			this.command.Connection = connection;
			if (!sql.Contains(' '))
				command.CommandType = CommandType.StoredProcedure;

			if (args == null)
				return;

			this.parameters = ParameterFactory.Create(args);

			List<IDataParameter> items = this.parameters.CreateParameters();
			foreach (IDataParameter item in items)
			{
				object value = item.Value ?? DBNull.Value;
				SqlParameter _parameter = NewParameter("@" + item.ParameterName, value, item.Direction);
				command.Parameters.Add(_parameter);
			}
		}

		private SqlParameter NewParameter(string parameterName, object value, ParameterDirection direction)
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


		public override int ExecuteNonQuery()
		{
			try
			{
				connection.Open();
				int count = command.ExecuteNonQuery();
				parameters?.UpdateResult(command.Parameters.Cast<IDataParameter>());
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

	}
}

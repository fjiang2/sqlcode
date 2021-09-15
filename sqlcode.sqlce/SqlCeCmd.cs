using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlServerCe;

namespace Sys.Data.SqlCe
{
	public class SqlCeCmd : BaseDbCmd, IDbCmd
	{
		private readonly SqlCeCommand command;
		private readonly SqlCeConnection connection;

		private readonly string[] statements;
		private readonly IParameterFactory parameters;

		public SqlCeCmd(SqlCeConnectionStringBuilder connectionString, string sql, object args)
			: this(connectionString, new SqlUnit(sql, args))
		{
		}

		public SqlCeCmd(SqlCeConnectionStringBuilder connectionString, SqlUnit unit)
		{
			this.statements = unit.Statements;
			object args = unit.Arguments;

			this.command = new SqlCeCommand();
			this.connection = new SqlCeConnection(connectionString.ConnectionString);
			this.command.Connection = connection;

			if (args == null)
				return;

			this.parameters = ParameterFactory.Create(args);

			List<IDataParameter> items = this.parameters.CreateParameters();
			foreach (IDataParameter item in items)
			{
				object value = item.Value ?? DBNull.Value;
				SqlCeParameter _parameter = NewParameter("@" + item.ParameterName, value, item.Direction);
				command.Parameters.Add(_parameter);
			}
		}

		private SqlCeParameter NewParameter(string parameterName, object value, ParameterDirection direction)
		{
			DbType dbType = DbType.AnsiString;
			if (value is int)
				dbType = DbType.Int32;
			else if (value is short)
				dbType = DbType.Int16;
			else if (value is long)
				dbType = DbType.Int64;
			else if (value is byte)
				dbType = DbType.Byte;
			else if (value is DateTime)
				dbType = DbType.DateTime;
			else if (value is double)
				dbType = DbType.Double;
			else if (value is float)
				dbType = DbType.Single;
			else if (value is decimal)
				dbType = DbType.Decimal;
			else if (value is bool)
				dbType = DbType.Boolean;
			else if (value is string && ((string)value).Length > 4000)
				dbType = DbType.AnsiString;
			else if (value is string)
				dbType = DbType.String;
			else if (value is byte[])
				dbType = DbType.Binary;
			else if (value is Guid)
				dbType = DbType.Guid;

			SqlCeParameter param = new SqlCeParameter(parameterName, dbType)
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
				int count = 0;
				foreach (string statement in statements)
				{
					command.CommandText = statement;
					SqlCeDataAdapter adapter = new SqlCeDataAdapter(command);
					DataTable dt = new DataTable();
					count += adapter.Fill(dt);
					dataSet.Tables.Add(dt);
				}
				return count;
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
				SqlCeDataAdapter adapter = new SqlCeDataAdapter(command);
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

				int count = 0;
				foreach (string statement in statements)
				{
					command.CommandText = statement;
					count += command.ExecuteNonQuery();
					parameters?.UpdateResult(command.Parameters.Cast<IDataParameter>());
				}

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

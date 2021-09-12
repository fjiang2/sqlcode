using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;

namespace Sys.Data
{
	public class SQLiteAgent : IDbAgent
	{
		public string ConnectionString { get; }

		public SQLiteAgent(string connectionStirng)
		{
			this.ConnectionString = connectionStirng;
		}

		public IDbCmd Command(string sql, object args) 
			=> new SQLiteCmd(new SQLiteConnectionStringBuilder(ConnectionString), sql, args);

		public DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SQLite };
		public DbCmdFunction Function => Command;

	}

	public class SQLiteCmd : BaseDbCmd, IDbCmd
	{
		private SQLiteCommand command;
		private SQLiteConnection connection;
		private IParameterFactory parameters;

		public SQLiteCmd(SQLiteConnectionStringBuilder connectionString, string sql, object args)
		{
			this.command = new SQLiteCommand(sql);
			this.connection = new SQLiteConnection(connectionString.ConnectionString);
			this.command.Connection = connection;

			if (args == null)
				return;

			if (args is string)
			{
				//The parameters could be JSON
				return;
			}

			this.parameters = ParameterFactory.Create(args);

			List<IDataParameter> items = this.parameters.CreateParameters();
			foreach (IDataParameter item in items)
			{
				object value = item.Value ?? DBNull.Value;
				SQLiteParameter parameter = NewParameter("@" + item.ParameterName, value, item.Direction);
				command.Parameters.Add(parameter);
			}
		}

		private SQLiteParameter NewParameter(string parameterName, object value, ParameterDirection direction)
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

			SQLiteParameter param = new SQLiteParameter(parameterName, dbType)
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
				SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
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
				SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
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
				int n = command.ExecuteNonQuery();
				parameters?.UpdateResult(command.Parameters.Cast<IDataParameter>());
				return n;
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

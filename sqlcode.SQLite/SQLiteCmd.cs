using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using Sys.Data;

namespace sqlcode.SQLite
{
	public class SQLiteCmd : BaseDbCmd, IDbCmd
	{
		private SQLiteConnectionStringBuilder connectionString;
		private SQLiteCommand command;
		private SQLiteConnection connection;
		private IParameterFactory parameters;

		public SQLiteCmd(SQLiteConnectionStringBuilder connectionString, string sql, object args)
		{
			this.connectionString = connectionString;
			this.command = new SQLiteCommand(sql);
			this.connection = new SQLiteConnection(connectionString.ConnectionString);
			this.command.Connection = connection;
			PrepareParameters(args);
		}

		private void PrepareParameters(object args)
		{
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

		public void AddOutParameterOfIdentity(string parameterName)
		{
			SQLiteParameter parameter = NewParameter($"@{parameterName}", 0, ParameterDirection.Output);
			command.Parameters.Add(parameter);
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


		public override DataSet FillDataSet(DataSet ds)
		{
			try
			{
				connection.Open();
				SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
				adapter.Fill(ds);
				return ds;
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Data.SQLite;

namespace Sys.Data.SQLite
{

	public class SQLiteAccess : DbAccess, IDbAccess
	{
		private const string connectionStringFormat = "provider=sqlite;Data Source={0};Version=3; DateTimeFormat=Ticks; Pooling=True; Max Pool Size=100;";

		private readonly SQLiteCommand command;
		private readonly SQLiteConnection connection;

		private readonly string[] statements;
		private readonly IParameterFacet facet;

		public SQLiteAccess(string fileName, string sql, object args)
			: this(new SQLiteConnectionStringBuilder(string.Format(connectionStringFormat, fileName)), new SqlUnit(sql, args))
		{
		}

		public SQLiteAccess(SQLiteConnectionStringBuilder connectionString, string sql, object args)
			: this(connectionString, new SqlUnit(sql, args))
		{
		}

		public SQLiteAccess(SQLiteConnectionStringBuilder connectionString, SqlUnit unit)
		{
			this.statements = unit.Statements;
			object args = unit.Arguments;

			this.command = new SQLiteCommand();
			this.connection = new SQLiteConnection(connectionString.ConnectionString);
			this.command.Connection = connection;

			if (args == null)
				return;

			this.facet = ParameterFacet.Create(args);

			List<IDataParameter> parameters = this.facet.CreateParameters();
			foreach (IDataParameter parameter in parameters)
			{
				object value = parameter.Value ?? DBNull.Value;
				SQLiteParameter _parameter = NewParameter("@" + parameter.ParameterName, value, parameter.Direction);
				command.Parameters.Add(_parameter);
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

				int count = 0;
				foreach (string statement in statements)
				{
					command.CommandText = statement;
					SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
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
				command.CommandText = statements.FirstOrDefault();
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

				int count = 0;
				foreach (string statement in statements)
				{
					command.CommandText = statement;
					count += command.ExecuteNonQuery();
					facet?.UpdateResult(command.Parameters.Cast<IDataParameter>());
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


		public override void ExecuteTransaction()
		{
			if (statements.Count() == 0)
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

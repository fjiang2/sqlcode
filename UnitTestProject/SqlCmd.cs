using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Sys.Data;
using Sys.Data.Text;

namespace UnitTestProject
{
	public class SqlCmd : BaseDbCmd, IDbCmd
	{
		private SqlConnectionStringBuilder connectionString;
		private SqlCommand command;
		private SqlConnection connection;
		private IParameterFactory factory;

		public SqlCmd(SqlConnectionStringBuilder connectionString, string sql, object parameters)
		{
			this.connectionString = connectionString;
			this.command = new SqlCommand(sql);
			this.connection = new SqlConnection(connectionString.ConnectionString);
			this.command.Connection = connection;

			PrepareParameters(parameters);
		}

		private void PrepareParameters(object parameters)
		{
			if (parameters == null)
				return;

			if (parameters is string)
			{
				//The parameters could be JSON
				return;
			}

			if (parameters is List<IDataParameter> list)
				factory = new ListParameters(list);
			else if (parameters is IDictionary<string, object> dict)
				factory = new DictionaryParameters(dict);
			else
				factory = new ObjectParameters(parameters);

			List<IDataParameter> items = factory.Create();
			foreach (IDataParameter item in items)
			{
				object value = item.Value ?? DBNull.Value;
				SqlParameter parameter = NewParameter("@" + item.ParameterName, value, item.Direction);
				command.Parameters.Add(parameter);
			}
		}

		public void AddOutParameterOfIdentity(string parameterName)
		{
			SqlParameter parameter = NewParameter($"@{parameterName}", 0, ParameterDirection.Output);
			command.Parameters.Add(parameter);
		}

		private void CompleteParameters()
		{
			if (factory == null)
				return;

			factory.Update(command.Parameters.Cast<IDataParameter>());
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


		public override DataSet FillDataSet(DataSet ds)
		{
			try
			{
				connection.Open();
				SqlDataAdapter adapter = new SqlDataAdapter(command);
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
				CompleteParameters();
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

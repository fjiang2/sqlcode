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
	public class SqlCmd : BaseDbCmd
	{
		private SqlConnectionStringBuilder connectionString;
		private SqlCommand command;
		private SqlConnection connection;
		private object parameters;

		public SqlCmd(SqlConnectionStringBuilder connectionString, string sql, object parameters)
		{
			this.connectionString = connectionString;
			this.command = new SqlCommand(sql);
			this.connection = new SqlConnection(connectionString.ConnectionString);
			this.command.Connection = connection;
			this.parameters = parameters;

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
				foreach (IDataParameter item in list)
				{
					object value = item.Value ?? DBNull.Value;
					SqlParameter parameter = NewParameter("@" + item.ParameterName, value, item.Direction);
					command.Parameters.Add(parameter);
				}
			else if (parameters is IDictionary<string, object> dict)
				foreach (KeyValuePair<string, object> item in dict)
				{
					object value = item.Value ?? DBNull.Value;
					SqlParameter parameter = NewParameter("@" + item.Key, value, ParameterDirection.Input);
					command.Parameters.Add(parameter);
				}
			else
				foreach (var propertyInfo in parameters.GetType().GetProperties())
				{
					object value = propertyInfo.GetValue(parameters) ?? DBNull.Value;
					SqlParameter parameter = NewParameter("@" + propertyInfo.Name, value, ParameterDirection.Input);
					command.Parameters.Add(parameter);
				}
		}


		private void CompleteParameters()
		{
			if (parameters == null)
				return;

			foreach (IDataParameter parameter in command.Parameters)
			{
				//skip letter '@'
				string parameterName = parameter.ParameterName.Substring(1);

				if (parameter.Direction != ParameterDirection.Input)
				{
					if (parameters is List<IDataParameter> list)
					{
						var result = list.Find(x => x.ParameterName == parameterName);
						if (result != null)
							result.Value = parameter.Value;
					}
					else if (parameter is IDictionary<string, object> dict)
					{
						if (dict.ContainsKey(parameterName))
						{
							dict[parameterName] = parameter.Value;
						}
					}
					else
					{
						var result = parameters.GetType().GetProperties().FirstOrDefault(property => property.Name == parameterName);
						if (result != null)
						{
							result.SetValue(parameters, parameter.Value);
						}
					}
				}
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

		public int GetIdentity()
		{
			var cmd = new SqlCmd(this.connectionString, SqlTemplate.SetIdentityOutParameter("@Identity"), null);
			SqlParameter parameter = cmd.NewParameter("@Identity", 0, ParameterDirection.Output);
			cmd.command.Parameters.Add(parameter);
			cmd.ExecuteNonQuery();
			return Convert.ToInt32(parameter.Value);
		}
	}
}

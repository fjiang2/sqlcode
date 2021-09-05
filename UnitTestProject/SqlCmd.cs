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
		private SqlCommand cmd;
		private SqlConnection conn;
		private object parameters;

		public SqlCmd(SqlConnectionStringBuilder connectionString, string sql, object parameters)
		{
			this.cmd = new SqlCommand(sql);
			this.conn = new SqlConnection(connectionString.ConnectionString);
			this.cmd.Connection = conn;
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
					cmd.Parameters.Add(parameter);
				}
			else if (parameters is IDictionary<string, object> dict)
				foreach (KeyValuePair<string, object> item in dict)
				{
					object value = item.Value ?? DBNull.Value;
					SqlParameter parameter = NewParameter("@" + item.Key, value, ParameterDirection.Input);
					cmd.Parameters.Add(parameter);
				}
			else
				foreach (var propertyInfo in parameters.GetType().GetProperties())
				{
					object value = propertyInfo.GetValue(parameters) ?? DBNull.Value;
					SqlParameter parameter = NewParameter("@" + propertyInfo.Name, value, ParameterDirection.Input);
					cmd.Parameters.Add(parameter);
				}
		}


		private void CompleteParameters()
		{
			if (parameters == null)
				return;

			foreach (IDataParameter parameter in cmd.Parameters)
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
				conn.Open();
				SqlDataAdapter adapter = new SqlDataAdapter(cmd);
				adapter.Fill(ds);
				return ds;
			}
			finally
			{
				conn.Close();
			}
		}

		public override int ExecuteNonQuery()
		{
			try
			{
				conn.Open();
				int n = cmd.ExecuteNonQuery();
				CompleteParameters();
				return n;
			}
			finally
			{
				conn.Close();
			}
		}


		public override object ExecuteScalar()
		{
			try
			{
				conn.Open();
				return cmd.ExecuteScalar();
			}
			finally
			{
				conn.Close();
			}
		}

	}
}

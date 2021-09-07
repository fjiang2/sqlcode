//--------------------------------------------------------------------------------------------------//
//                                                                                                  //
//        DPO(Data Persistent Object)                                                               //
//                                                                                                  //
//          Copyright(c) Datum Connect Inc.                                                         //
//                                                                                                  //
// This source code is subject to terms and conditions of the Datum Connect Software License. A     //
// copy of the license can be found in the License.html file at the root of this distribution. If   //
// you cannot locate the  Datum Connect Software License, please send an email to                   //
// datconn@gmail.com. By using this source code in any fashion, you are agreeing to be bound        //
// by the terms of the Datum Connect Software License.                                              //
//                                                                                                  //
// You must not remove this notice, or any other, from this software.                               //
//                                                                                                  //
//                                                                                                  //
//--------------------------------------------------------------------------------------------------//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Sys.Data.Text
{
	public class ParameterContext
	{
		//<name, parameter>
		private readonly Dictionary<string, IDataParameter> parameters = new Dictionary<string, IDataParameter>();

		public ParameterContext()
		{
		}

		public ParameterContext(IDictionary<string, object> dict)
		{
			foreach (KeyValuePair<string, object> item in dict)
			{
				var parameter = new Parameter(item.Key, item.Value)
				{
					Direction = ParameterDirection.Input,
				};

				AddParameter(parameter);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args">instance of property class</param>
		public ParameterContext(object args)
		{
			foreach (var propertyInfo in args.GetType().GetProperties())
			{
				var parameter = new Parameter(propertyInfo.Name, propertyInfo.GetValue(args))
				{
					Direction = ParameterDirection.Input,
				};

				AddParameter(parameter);
			}
		}

		public void Clear()
		{
			parameters.Clear();
		}

		/// <summary>
		/// A list of all parameters
		/// </summary>
		public List<IDataParameter> Parameters => this.parameters.Values.ToList();


		public IDataParameter this[string parameterName]
		{
			get
			{
				if (!parameters.ContainsKey(parameterName))
					throw new InvalidExpressionException("invalid parameter name");

				return parameters[parameterName];
			}
			set
			{
				AddParameter(value);
			}
		}

		/// <summary>
		/// Add a parameter which can be SqlParameter, OleDbParameter
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public IDataParameter AddParameter(IDataParameter parameter)
		{
			if (parameters.ContainsKey(parameter.ParameterName))
				parameters.Remove(parameter.ParameterName);

			parameters.Add(parameter.ParameterName, parameter);

			return parameter;
		}

		public IDataParameter GetParameter(string parameterName)
		{
			if (parameters.ContainsKey(parameterName))
				return parameters[parameterName];

			return null;
		}

		public void RemoveParameter(string parameterName)
		{
			if (parameters.ContainsKey(parameterName))
				parameters.Remove(parameterName);
		}

		public override string ToString()
		{
			return parameters.ToString();
		}
	}
}

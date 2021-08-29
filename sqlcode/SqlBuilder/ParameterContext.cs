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

namespace Sys.Data.Text
{
	public class ParameterContext
	{
		//<parameter, column>
		private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

		public ParameterContext()
		{
		}

		public IDictionary<string, object> Parameters => this.parameters;

		public ParameterName CreateParameter(string parameterName, object value)
		{
			if (!parameters.ContainsKey(parameterName))
				parameters.Add(parameterName, value);

			return new ParameterName(parameterName);
		}

		public ParameterName CreateParameter(string parameterName)
		{
			return CreateParameter(parameterName, null);
		}

		public override string ToString()
		{
			return parameters.ToString();
		}
	}
}

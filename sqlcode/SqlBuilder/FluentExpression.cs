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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Sys.Data.Text
{
	/// <summary>
	/// 
	/// </summary>
	public static class FluentExpression
	{
		/// <summary>
		/// Convert object value to Expression
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression AsValue(this object value)
		{
			return new Expression(new SqlValue(value));
		}

		/// <summary>
		/// Create  expression of variable 
		/// </summary>
		/// <param name="variableName"></param>
		/// <returns></returns>
		public static Expression AsVariable(this string variableName)
		{
			return new Expression(new VariableName(variableName));
		}

		/// <summary>
		/// Assing value to variable: variable = value
		/// </summary>
		/// <param name="variableName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression AssignVarible(this string variableName, object value)
		{
			return variableName.AsVariable().LET(value);
		}

		/// <summary>
		/// Create parameter: @parameterName with value
		/// </summary>
		/// <param name="context"></param>
		/// <param name="parameterName">name of parameter</param>
		/// <param name="value">the value of parameter</param>
		/// <param name="direction">the parameter directior: in,out,in/out,return</param>
		/// <returns></returns>
		public static Expression AsParameter(this ParameterContext context, string parameterName, object value = null, ParameterDirection direction = ParameterDirection.Input)
		{
			var parameter = new Parameter(parameterName, value)
			{
				Direction = direction,
			};

			return AsParameter(context, parameter);
		}

		/// <summary>
		/// Create out parameter. Value is used to determine parameter type 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="parameterName"></param>
		/// <param name="value">Used to determine parameter type</param>
		/// <returns></returns>
		public static Expression AsOutParameter(this ParameterContext context, string parameterName, object value)
		{
			var parameter = new Parameter(parameterName, value)
			{
				Direction = ParameterDirection.Output,
			};

			return AsParameter(context, parameter);
		}

		/// <summary>
		/// Create ref parameter
		/// </summary>
		/// <param name="context"></param>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression AsRefParameter(this ParameterContext context, string parameterName, object value)
		{
			var parameter = new Parameter(parameterName, value)
			{
				Direction = ParameterDirection.InputOutput,
			};

			return AsParameter(context, parameter);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static Expression AsParameter(this ParameterContext context, IDataParameter parameter)
		{
			context.AddParameter(parameter);
			return new Expression(new ParameterName(parameter.ParameterName));
		}

		/// <summary>
		/// Used to define stored procedure
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Expression AsParameter(this string parameterName, string type = null)
		{
			var arg = new Expression(new ParameterName(parameterName));
			if (type != null)
				arg = arg.PARAMETER(type);

			return arg;
		}

		/// <summary>
		/// Create expression of  column name: "name" -> [name]
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public static Expression AsColumn(this string columnName)
		{
			if (columnName == "*")
				return Expression.STAR;

			return new Expression(new ColumnName(columnName));
		}

		/// <summary>
		/// Create expression of column name:  [Categories].[CategoryID], C.[CategoryID]
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>

		public static Expression AsColumn(this string columnName, string tableName)
		{
			return new Expression(new ColumnName(tableName, columnName));
		}

		/// <summary>
		/// Convert column name value to Expression
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static Expression AsColumn(this string columnName, ITableName tableName)
		{
			return new Expression(new ColumnName(tableName.FullName, columnName));
		}

		/// <summary>
		/// Create an instance of ITableName
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="schemaName"></param>
		/// <returns></returns>
		public static ITableName AsTableName(this string tableName, string schemaName = null)
		{
			return new TableName(schemaName, tableName);
		}

		/// <summary>
		/// Assing value to column: [column-name] = value
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression AssignColumn(this string columnName, object value)
		{
			return columnName.AsColumn().LET(value);
		}


		/// <summary>
		/// Create function call
		/// </summary>
		/// <param name="function"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static Expression AsFunction(this string function, params Expression[] args)
		{
			return Expression.Function(function, args);
		}


		/// <summary>
		/// Create AND operation chain
		/// </summary>
		/// <param name="exprList"></param>
		/// <returns></returns>
		public static Expression AND(this IEnumerable<Expression> exprList)
		{
			return Expression.AND(exprList.ToArray());
		}

		/// <summary>
		/// Create OR operation chain
		/// </summary>
		/// <param name="exprList"></param>
		/// <returns></returns>
		public static Expression OR(this IEnumerable<Expression> exprList)
		{
			return Expression.OR(exprList.ToArray());
		}


		/// <summary>
		/// Create SQL: EXISTS(SELECT * FROM Products)
		/// </summary>
		/// <param name="select"></param>
		/// <returns></returns>
		public static Expression EXISTS(this SqlBuilder select)
		{
			return Expression.EXISTS(select);
		}
	}
}

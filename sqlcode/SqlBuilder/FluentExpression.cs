﻿//--------------------------------------------------------------------------------------------------//
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
using System.Text;

namespace Sys.Data.Text
{
	public static class FluentExpression
	{
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
		/// <returns></returns>
		public static Expression AsParameter(this ParameterContext context, string parameterName, object value = null)
		{
			return new Expression(context.CreateParameter(parameterName, value));
		}

		/// <summary>
		/// Create expression of  column name: "name" -> [name]
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public static Expression AsColumn(this string columnName)
		{
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

		/// <summary>
		/// Create function call
		/// </summary>
		/// <param name="func"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static Expression Function(this string func, params Expression[] args)
		{
			return Expression.Function(func, args);
		}

	}
}
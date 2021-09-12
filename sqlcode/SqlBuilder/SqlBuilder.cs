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
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Sys.Data.Text
{

	/// <summary>
	/// SQL clauses builder
	/// </summary>
	public sealed class SqlBuilder : IQueryScript
	{

		CodeBlock block = new CodeBlock();
		public SqlBuilder()
		{
		}

		public string ToScript(DbProviderStyle style)
		{
			return block.ToScript(style);
		}

		/// <summary>
		/// Append any code
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public SqlBuilder Append(string text)
		{
			block.Append(text);
			return this;
		}

		public SqlBuilder AppendLine()
		{
			block.AppendLine();
			return this;
		}

		public SqlBuilder Append(SqlBuilder builder)
		{
			block.Append(builder);
			return this;
		}

		public SqlBuilder Append(Expression expr)
		{
			block.Append(expr);
			return this;
		}

		private SqlBuilder AppendSpace(string text)
		{
			block.AppendSpace(text);
			return this;
		}

		private SqlBuilder AppendSpace()
		{
			block.AppendSpace();
			return this;
		}

		private SqlBuilder WithTableName(string keyword, string tableName, string alias)
		{
			AppendSpace(keyword);

			int start = tableName.IndexOf("[");
			int stop = tableName.IndexOf("]");
			if (start >= 0 && stop > 0 && start < stop)
				AppendSpace(tableName);
			else
				AppendSpace(new TableName(tableName).FullName);

			if (!string.IsNullOrEmpty(alias))
				AppendSpace(alias);

			return this;
		}

		public SqlBuilder USE(string database)
		{
			return AppendSpace($"USE {database}");
		}


		#region SELECT clause

		public SqlBuilder SELECT() => AppendSpace("SELECT");

		public SqlBuilder DISTINCT(bool nop = false) => !nop ? AppendSpace("DISTINCT") : this;

		public SqlBuilder ALL(bool nop = false) => !nop ? AppendSpace("ALL") : this;

		public SqlBuilder TOP(int n)
		{
			if (n > 0)
				return AppendSpace($"TOP {n}");

			return this;
		}

		public SqlBuilder LIMIT(int n)
		{
			if (n > 0)
				return AppendSpace($"LIMIT {n}");

			return this;
		}

		public SqlBuilder OFFSET(int n)
		{
			if (n > 0)
				return AppendSpace($"OFFSET {n}");

			return this;
		}

		/// <summary>
		/// Any columns
		/// </summary>
		/// <param name="columns"></param>
		/// <returns></returns>
		public SqlBuilder COLUMNS(string columns)
		{
			return AppendSpace(columns);
		}

		public SqlBuilder COLUMNS()
		{
			return COLUMNS("*");
		}

		public SqlBuilder COLUMNS(params Expression[] columns)
		{
			return COLUMNS((IEnumerable<Expression>)columns);
		}

		public SqlBuilder COLUMNS(IEnumerable<Expression> columns)
		{
			if (columns.Count() == 0)
				return COLUMNS("*");
			else
				return Append(new Expression(columns)).AppendSpace();
		}

		public SqlBuilder COLUMNS(IEnumerable<string> columns)
		{
			if (columns.Count() == 0)
				return COLUMNS("*");
			else
				return COLUMNS(JoinColumns(columns));
		}

		#endregion

		public SqlBuilder FROM(ITableName from, string alias = null) => FROM(from.FullName, alias);
		public SqlBuilder FROM(string from, string alias = null) => WithTableName("FROM", from, alias);


		public SqlBuilder UPDATE(ITableName tableName, string alias = null) => UPDATE(tableName.FullName, alias);
		public SqlBuilder UPDATE(string tableName, string alias = null)
		{
			return WithTableName("UPDATE", tableName, alias);
		}

		public SqlBuilder SET(IEnumerable<Expression> assignments) => AppendSpace("SET").Append(new Expression(assignments)).Append(" ");
		public SqlBuilder SET(params Expression[] assignments) => SET((IEnumerable<Expression>)assignments);

		public SqlBuilder INSERT_INTO(ITableName tableName) => INSERT_INTO(tableName.FullName);
		public SqlBuilder INSERT_INTO(string tableName)
		{
			WithTableName("INSERT INTO", tableName, null);
			return this;
		}

		public SqlBuilder INSERT_INTO(ITableName tableName, IEnumerable<Expression> columns) => INSERT_INTO(tableName.FullName, columns);
		public SqlBuilder INSERT_INTO(string tableName, IEnumerable<Expression> columns)
		{
			WithTableName("INSERT INTO", tableName, null);

			if (columns.Count() > 0)
				Append("(").Append(new Expression(columns)).AppendSpace(")");

			return this;
		}

		public SqlBuilder INSERT_INTO(ITableName tableName, IEnumerable<string> columns) => INSERT_INTO(tableName.FullName, columns);
		public SqlBuilder INSERT_INTO(string tableName, IEnumerable<string> columns)
		{
			WithTableName("INSERT INTO", tableName, null);

			if (columns.Count() > 0)
				AppendSpace($"({JoinColumns(columns)})");

			return this;
		}

		public SqlBuilder VALUES(params Expression[] values)
		{
			return Append("VALUES (").Append(new Expression(values)).Append(")");
		}

		public SqlBuilder VALUES(params object[] values)
		{
			return VALUES((IEnumerable<object>)values);
		}

		public SqlBuilder VALUES(IEnumerable<object> values)
		{
			var L = values.Select(x => new Expression(new SqlValue(x))).ToArray();
			return AppendSpace($"VALUES ({string.Join<Expression>(", ", L)})");
		}

		public SqlBuilder DELETE_FROM(ITableName tableName) => DELETE_FROM(tableName.FullName);
		public SqlBuilder DELETE_FROM(string tableName)
		{
			return WithTableName("DELETE FROM", tableName, null);
		}

		/// <summary>
		/// skip WHERE if expr is null
		/// </summary>
		/// <param name="expr"></param>
		/// <returns></returns>
		public SqlBuilder WHERE(Expression expr)
		{
			if (expr is null)
				return this;

			return AppendSpace("WHERE").Append(expr).AppendSpace();
		}

		public SqlBuilder WHERE(ILocator locator)
		{
			if (locator is null)
				return this;

			return WHERE(locator.Where);
		}

		/// <summary>
		/// skip WHERE if expr is null or empty
		/// </summary>
		/// <param name="condition"></param>
		/// <returns></returns>
		public SqlBuilder WHERE(string condition)
		{
			if (!string.IsNullOrEmpty(condition))
				AppendSpace($"WHERE {condition}");

			return this;
		}


		#region INNER/OUT JOIN clause

		public SqlBuilder LEFT() => AppendSpace("LEFT");

		public SqlBuilder RIGHT() => AppendSpace("RIGHT");

		public SqlBuilder INNER() => AppendSpace("INNER");

		public SqlBuilder OUTTER() => AppendSpace("OUTTER");
		public SqlBuilder FULL() => AppendSpace("FULL");

		public SqlBuilder JOIN(ITableName tableName, string alias = null) => JOIN(tableName.FullName, alias);
		public SqlBuilder JOIN(string tableName, string alias = null) => WithTableName("JOIN", tableName, alias);

		public SqlBuilder ON(Expression expr)
		{
			AppendSpace("ON").Append(expr).AppendSpace();
			return this;
		}

		#endregion



		#region GROUP BY / HAVING clause
		public SqlBuilder GROUP_BY(params Expression[] columns)
		{
			if (columns == null || columns.Length == 0)
				return this;

			return AppendSpace("GROUP BY").Append(new Expression(columns)).AppendSpace();
		}

		public SqlBuilder GROUP_BY(params string[] columns)
		{
			if (columns == null || columns.Length == 0)
				return this;

			return AppendSpace($"GROUP BY {JoinColumns(columns)}");
		}

		public SqlBuilder HAVING(Expression condition)
		{
			return AppendSpace($"HAVING").Append(condition).AppendSpace();
		}

		#endregion



		public SqlBuilder ORDER_BY(params Expression[] columns)
		{
			if (columns == null || columns.Length == 0)
				return this;

			return AppendSpace("ORDER BY").Append(new Expression(columns)).AppendSpace(); 
		}

		public SqlBuilder ORDER_BY(params string[] columns)
		{
			if (columns == null || columns.Length == 0)
				return this;

			return AppendSpace($"ORDER BY {JoinColumns(columns)}");
		}

		public SqlBuilder UNION() => AppendSpace("UNION");
		public SqlBuilder DESC() => AppendSpace("DESC");
		public SqlBuilder ASC() => AppendSpace("ASC");

		public SqlBuilder INTO(ITableName tableName) => INTO(tableName.FullName);
		public SqlBuilder INTO(string tableName) => WithTableName("INTO", tableName, null);

		public SqlBuilder ALTER() => AppendSpace("ALTER");
		public SqlBuilder CREATE() => AppendSpace("CREATE");
		public SqlBuilder DROP() => AppendSpace("DROP");

		private static string JoinColumns(IEnumerable<string> columns)
		{
			return string.Join(", ", columns.Select(x => new ColumnName(x)));
		}

		public string ToString(DbProviderStyle style)
		{
			return ToScript(style);
		}

		public override string ToString()
		{
			return ToScript(DbProviderOption.DefaultStyle);
		}
	}
}

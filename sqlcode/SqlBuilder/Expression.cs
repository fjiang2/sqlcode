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
using System.Text;

namespace Sys.Data.Text
{
	public partial class Expression : IQueryScript
	{
		public static readonly Expression NULL = new Expression("NULL");
		public static readonly Expression STAR = new Expression("*");
		public static readonly Expression IDENTITY = new Expression("@@IDENTITY");
		public static readonly Expression ERROR = new Expression("@@ERROR");


		private readonly List<object> script = new List<object>();

		/// <summary>
		/// Compound expression
		/// </summary>
		internal bool Compound = false;

		internal Expression()
		{
		}

		internal Expression(VariableName name)
		{
			script.Add(name);
		}

		internal Expression(ColumnName name)
		{
			script.Add(name);
		}

		internal Expression(ParameterName name)
		{
			script.Add(name);
		}

		internal Expression(SqlValue value)
		{
			script.Add(value);
		}

		private Expression(Expression expr)
		{
			script.AddRange(expr.script);
			this.Compound = expr.Compound;
		}

		internal Expression(IEnumerable<Expression> exprList)
		{
			Append(exprList);
		}

		private Expression(string text)
		{
			script.Add(text);
		}

		public Expression this[Expression expr] => this.Append("[").Append(expr).Append("]");

		private Expression Append(IEnumerable<Expression> list)
		{
			list.ForEach(x => Append(x), _ => Append(", "));
			return this;
		}

		private Expression Append(Expression expr)
		{
			script.Add(expr);
			return this;
		}

		private Expression Append(VariableName name)
		{
			script.Add(name);
			return this;
		}

		protected Expression Append(string text)
		{
			script.Add(text);
			return this;
		}

		protected Expression Append(object obj)
		{
			script.Add(obj);
			return this;
		}


		private Expression AppendSpace(string text) => Append(text).AppendSpace();
		private Expression WrapSpace(string text) => AppendSpace().Append(text).AppendSpace();
		private Expression AffixSpace(string text) => AppendSpace().Append(text);
		private Expression AppendSpace() => Append(" ");

		/// <summary>
		/// Assignment: name = value
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression LET(Expression name, Expression value) => new BinaryExpression(name, "=", value);
		public static Expression LET(Expression name, object value)
		{
			if (value is Expression expr)
				return LET(name, expr);
			else
				return LET(name, new Expression(new SqlValue(value)));
		}

		public Expression LET(object value) => LET(this, value);
		public Expression LET() => new Expression(this).WrapSpace("=");

		public Expression AS(VariableName name) => new Expression(this).WrapSpace("AS").Append(name);

		public Expression IN(SqlBuilder select) => new Expression(this).WrapSpace("IN").Append("(").Append(select).Append(")");
		public Expression NOT_IN(SqlBuilder select) => new Expression(this).WrapSpace("NOT IN").Append("(").Append(select).Append(")");

		private static Expression IN__NOT_IN(Expression expr, string opr, IEnumerable<Expression> collection)
		{
			if (collection == null || collection.Count() == 0)
				return expr;

			return new BinaryExpression(expr, opr, new Expression(collection) { Compound = true });
		}

		public Expression IN(params Expression[] collection) => IN__NOT_IN(this, "IN", collection);
		public Expression NOT_IN(params Expression[] collection) => IN__NOT_IN(this, "NOT IN", collection);
		public Expression IN<T>(IEnumerable<T> values) => IN__NOT_IN(this, "IN", values.Select(x => new Expression(new SqlValue(x))));
		public Expression NOT_IN<T>(IEnumerable<T> values) => IN__NOT_IN(this, "NOT IN", values.Select(x => new Expression(new SqlValue(x))));

		private static Expression BETWEEN(Expression expr, string opr, Expression expr1, Expression expr2) => new BinaryExpression(expr, opr, new BinaryExpression(expr1, "AND", expr2) { Compound = false });
		public Expression BETWEEN(Expression expr1, Expression expr2) => BETWEEN(this, "BETWEEN", expr1, expr2);
		public Expression NOT_BETWEEN(Expression expr1, Expression expr2) => BETWEEN(this, "NOT BETWEEN", expr1, expr2);
		public Expression BETWEEN<T>(T value1, T value2) => BETWEEN(this, "BETWEEN", new Expression(new SqlValue(value1)), new Expression(new SqlValue(value2)));
		public Expression NOT_BETWEEN<T>(T value1, T value2) => BETWEEN(this, "NOT BETWEEN", new Expression(new SqlValue(value1)), new Expression(new SqlValue(value2)));

		public Expression IS_NULL() => new Expression(this).AffixSpace("IS NULL");
		public Expression IS_NOT_NULL() => new Expression(this).AffixSpace("IS NOT NULL");


		public static Expression EXISTS(SqlBuilder select) => new Expression().Append("EXISTS (").Append(select).Append(")");


		public static Expression LIKE(Expression expr, Expression pattern) => new BinaryExpression(expr, "LIKE", pattern);
		public Expression LIKE(Expression pattern) => LIKE(this, pattern);

		public static Expression AND(params Expression[] exprList) => new BinaryExpression("AND", exprList);
		public static Expression AND(Expression expr1, Expression expr2) => new BinaryExpression(expr1, "AND", expr2);
		public Expression AND(Expression expr) => AND(this, expr);

		public static Expression OR(params Expression[] exprList) => new BinaryExpression("OR", exprList);
		public static Expression OR(Expression expr1, Expression expr2) => new BinaryExpression(expr1, "OR", expr2);
		public Expression OR(Expression expr) => OR(this, expr);

		public static Expression NOT(Expression expr) => new UnaryExpression("NOT", expr);
		public Expression NOT() => NOT(this);

		public string ToScript(DbAgentStyle style)
		{
			StringBuilder x = new StringBuilder();
			foreach (object item in script)
			{
				if (item is SqlValue value)
				{
					x.Append(value.ToScript(style));
				}
				else if (item is Expression expr)
				{
					x.Append(expr.ToScript(style));
				}
				else if (item is SqlBuilder sql)
				{
					x.Append(sql.ToScript(style));
				}
				else
				{
					x.Append(item);
				}
			}

			return x.ToString();
		}

		private bool IsNULL()
		{
			var x = script.SingleOrDefault();
			if (x == null)
				return false;

			if (x is SqlValue value)
			{
				return value.IsNull;
			}

			return false;
		}

		public override bool Equals(object obj)
		{
			return script.Equals(((Expression)obj).script);
		}

		public override int GetHashCode()
		{
			return script.GetHashCode();
		}

		public string ToString(DbAgentStyle style)
		{
			return ToScript(style);
		}

		public override string ToString()
		{
			return ToScript(DbAgentOption.DefaultStyle);
		}
	}
}

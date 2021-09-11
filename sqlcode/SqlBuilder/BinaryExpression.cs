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

namespace Sys.Data.Text
{
	class BinaryExpression : Expression
	{
		public Expression Left { get; }
		public Expression Right { get; }
		public string Method { get; }

		public BinaryExpression(Expression left, string method, Expression right)
		{
			this.Left = left;
			this.Right = right;
			this.Method = method;

			Expr2Str(Left);
			Append($" {Method} ");
			Expr2Str(Right);

			Compound = true; // left.Compound || right.Compound;
		}

		public BinaryExpression(string method, IEnumerable<Expression> exprList)
		{
			this.Method = method;
			exprList.ForEach(x => Expr2Str(x), _ => Append($" {Method} "));
			base.Compound = true;
		}

		private void Expr2Str(Expression expr)
		{
			if (expr.Compound)
			{
				Append("(");
				Append(expr);
				Append(")");
			}
			else
			{
				Append(expr);
			}
		}
	}
}

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
		}

		public Expression Reduce()
		{
			Expression expr = new Expression(string.Format("{0} {1} {2}", Expr2Str(Left), Method, Expr2Str(Right)))
			{
				Compound = true
			};

			return expr;
		}

		private static string Expr2Str(Expression expr)
		{
			return expr.Compound ? $"({expr})" : expr.ToString();
		}

		public override string ToString()
		{
			return Reduce().ToString();
		}
	}
}

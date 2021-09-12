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
	class UnaryExpression : Expression
	{
		public Expression Operand { get; }
		public string Method { get; }

		public UnaryExpression(string method, Expression operand)
		{
			this.Operand = operand;
			this.Method = method;

			base.Append($"{Method} ");
			Expr2Str(Operand);
			
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

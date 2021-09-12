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
using System.Text;

namespace Sys.Data.Text
{
	class CodeBlock
	{
		private readonly List<object> script = new List<object>();
		/// <summary>
		/// Append any code
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public CodeBlock Append(string text)
		{
			script.Add(text);
			return this;
		}

		public CodeBlock AppendLine()
		{
			script.Add(Environment.NewLine);
			return this;
		}

		public CodeBlock AppendLine(string text)
		{
			script.Add(text);
			script.Add(Environment.NewLine);
			return this;
		}

		public CodeBlock Append(Statement statement)
		{
			script.Add(statement);
			return this;
		}


		public CodeBlock Append(SqlBuilder builder)
		{
			script.Add(builder);
			return this;
		}

		public CodeBlock Append(Expression expr)
		{
			script.Add(expr);
			return this;
		}

		public CodeBlock AppendSpace(string text)
		{
			script.Add(text + " ");
			return this;
		}

		public CodeBlock AppendSpace()
		{
			script.Add(" ");
			return this;
		}

		public string ToScript(DbAgentStyle style)
		{
			List<string> lines = new List<string>();
			StringBuilder builder = new StringBuilder();

			foreach (object item in script)
			{
				if (item is Expression expr)
				{
					builder.Append(expr.ToScript(style));
				}
				else if (item is SqlBuilder sql)
				{
					builder.Append(sql.ToScript(style));
				}
				else if (item is Statement statement)
				{
					builder.Append(statement.ToScript(style));
				}
				else if (item is string str)
				{
					if (str == Environment.NewLine)
					{
						//remove extra space letter on each line
						lines.Add(builder.ToString().Trim());
						builder.Clear();
					}

					builder.Append(str);
				}
				else
					throw new Exception();
			}

			if (builder.Length > 0)
				lines.Add(builder.ToString().Trim());

			return string.Join(Environment.NewLine, lines);
		}
	}
}

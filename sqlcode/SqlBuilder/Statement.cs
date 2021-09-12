using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Text
{
	public class Statement : IQueryScript
	{
		private readonly CodeBlock block = new CodeBlock();
		protected bool compound = false;

		public static readonly Statement BREAK = new Statement().AppendLine("BREAK");
		public static readonly Statement CONTINUE = new Statement().AppendLine("CONTINUE");
		public static readonly Statement RETURN = new Statement().AppendLine("RETURN");

		public Statement()
		{
		}

		private Statement AppendLine(string line)
		{
			block.AppendLine(line);
			return this;
		}

		public Statement Append(SqlBuilder builder)
		{
			block.Append(builder);
			return this;
		}

		public Statement Compound(params Statement[] statements)
		{
			AppendLine("BEGIN");

			foreach (var statement in statements)
			{
				AppendLine(new string(' ', 2) + statement.ToString());
			}

			AppendLine("END");

			compound = true;

			return this;
		}

		public Statement IF(Expression condition, Statement then)
		{
			block.AppendSpace("IF").Append(condition).AppendSpace().Append(then);
			return this;
		}

		public Statement IF(Expression condition, Statement then, Statement _else)
		{
			block.AppendSpace("IF").Append(condition).AppendSpace().Append(then).AppendSpace().AppendSpace("ELSE").Append(_else);
			return this;
		}

		public Statement WHILE(Expression condition, Statement loop)
		{
			block.AppendSpace("WHILE").Append(condition).AppendSpace().Append(loop);
			return this;
		}

		public Statement DECLARE(VariableName name, Type type)
		{
			AppendLine($"DECLARE @{name} AS {type.SqlType()}");
			return this;
		}

		/// <summary>
		/// SET XXX ON / OFF
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Statement SET(string key, Expression value)
		{
			block.AppendSpace($"SET {key}").Append(value);
			return this;
		}

		/// <summary>
		/// SET @VAR = 12
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>  
		public Statement SET(VariableName name, Expression value)
		{
			block.AppendSpace($"SET").Append(new BinaryExpression(new Expression(name), "=", value));
			return this;
		}

		public Statement CREATE_FUNCTION(string functionName, Parameter result, params Parameter[] args)
		{
			string _args = string.Join<Parameter>(", ", args);
			AppendLine($"CREATE FUNCTION {functionName}({_args})");
			AppendLine($"RETURN {result}");
			return this;
		}

		public Statement CREATE_PROCEDURE(string procedureName, params Parameter[] args)
		{
			string _args = string.Join<Parameter>(", ", args);
			AppendLine($"CREATE PROCEDURE {procedureName}({_args})");
			AppendLine("AS");
			return this;
		}

		public string ToScript(DbProviderStyle style)
		{
			return block.ToScript(style);
		}

		public static implicit operator Statement(SqlBuilder sql)
		{
			return new Statement().Append(sql);
		}

		public override string ToString()
		{
			return ToScript(DbProviderOption.DefaultStyle);
		}
	}
}

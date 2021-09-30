using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Text
{
	public class Statement : IQueryScript
	{
		private readonly CodeBlock block = new CodeBlock();
		protected bool compound = false;


		public Statement()
		{
		}

		private Statement AppendLine(string line)
		{
			block.AppendLine(line);
			return this;
		}

		public Statement AppendLine()
		{
			block.AppendLine();
			return this;
		}

		public Statement AppendTab(int tab = 1)
		{
			if (tab <= 0)
				return this;

			block.Append(new string('\t', tab));
			return this;
		}

		public Statement Append(SqlBuilder builder)
		{
			block.Append(builder);
			return this;
		}

		public Statement Append(Statement statement)
		{
			block.Append(statement);
			return this;
		}

		public Statement Append(string statement)
		{
			block.Append(statement);
			return this;
		}

		public Statement Compound(params Statement[] statements)
		{
			AppendLine("BEGIN");

			foreach (var statement in statements)
			{
				AppendTab().Append(statement).AppendLine();
			}

			AppendLine("END");

			compound = true;

			return this;
		}

		public Statement BREAK() => Append("BREAK");
		public Statement CONTINUE() => Append("CONTINUE");
		public Statement RETURN() => Append("RETURN");


		public Statement IF(Expression condition)
		{
			block.AppendSpace("IF").Append(condition).AppendSpace();
			return this;
		}

		public Statement IF(Expression condition, Statement then)
		{
			block.AppendSpace("IF")
				.Append(condition)
				.AppendSpace()
				.Append(then);
			return this;
		}

		public Statement IF(Expression condition, Statement then, Statement _else)
		{
			block.AppendSpace("IF")
				.Append(condition).AppendSpace()
				.Append(then).AppendSpace()
				.AppendSpace("ELSE")
				.Append(_else);
			return this;
		}

		public Statement WHILE(Expression condition, Statement loop)
		{
			block.AppendSpace("WHILE")
				.Append(condition)
				.AppendSpace()
				.Append(loop);
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

		public Statement TRY_CATCH(Statement _try, Statement _catch)
		{
			block.AppendLine("BEGIN TRY")
				.Append(_try)
				.AppendLine()
				.AppendLine("END TRY")
				.AppendLine("BEGIN CATCH")
				.Append(_catch)
				.AppendLine()
				.AppendLine("END CATCH");

			return this;
		}

		public Statement BEGIN_TRANSACTION() => AppendLine("BEGIN TRANSACTION");
		public Statement ROLLBACK_TRANSACTION() => AppendLine("ROLLBACK TRANSACTION");
		public Statement COMMIT_TRANSACTION() => AppendLine("COMMIT TRANSACTION");
		public Statement GO() => AppendLine("GO");


		public string ToScript(DbAgentStyle style)
		{
			return block.ToScript(style);
		}

		public static implicit operator Statement(SqlBuilder sql)
		{
			return new Statement().Append(sql);
		}

		public override string ToString()
		{
			return ToScript(DbAgentOption.DefaultStyle);
		}
	}
}


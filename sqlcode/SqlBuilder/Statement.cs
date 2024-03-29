﻿using System;
using System.Collections.Generic;
using System.Linq;
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

		private Statement Append(string statement)
		{
			block.Append(statement);
			return this;
		}

		private Statement AppendSpace(string text)
		{
			block.AppendSpace(text);
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


		public Statement COMMENTS(string text) => Append("--").Append(text).AppendLine();

		//public Statement AS() => Append("AS");
		public Statement BREAK() => Append("BREAK");
		public Statement CONTINUE() => Append("CONTINUE");
		public Statement RETURN() => Append("RETURN");
		
		public Statement CREATE() => AppendSpace("CREATE");

		public Statement RETURN(Expression result)
		{
			block.AppendSpace("RETURN").Append(result);
			return this;
		}

		public Statement LET(Expression vname, Expression value)
		{
			block.AppendSpace("SET").Append(vname.LET(value));
			return this;
		}

		public Statement DECLARE(params Expression[] variables)
		{
			block.AppendSpace("DECLARE").Append(new Expression(variables));
			return this;
		}

		public Statement PROCEDURE(string name, params Expression[] args)
		{
			block.AppendSpace("PROCEDURE")
				.AppendSpace(name)
				.Append(new Expression(args))
				.AppendSpace()
				.Append("AS");
			return this;
		}

		public Statement FUNCTION(TYPE result, string name, params Expression[] args)
		{
			block.AppendSpace("FUNCTION")
				.Append(name)
				.Append(new Expression().TUPLE(args))
				.AppendSpace()
				.AppendSpace("RETURNS")
				.Append(new Expression(result)).AppendSpace().Append("AS");

			return this;
		}

		//public Statement RETURNS(TYPE type)
		//{
		//	block.AppendSpace("RETURNS").Append(new Expression(type));
		//	return this;
		//}

		public Statement EXECUTE(string name) => AppendSpace("EXECUTE").AppendSpace(name);
		public Statement EXECUTE(Expression result, string name)
		{
			block.AppendSpace("EXECUTE").Append(result).Append(" = ").AppendSpace(name);
			return this;
		}

		public Statement PARAMETERS(params Expression[] parameters)
		{
			return PARAMETERS((IEnumerable<Expression>)parameters);
		}

		public Statement PARAMETERS(IEnumerable<Expression> parameters)
		{
			if (parameters == null || parameters.Count() == 0)
				return this;
			
			block.Append(new Expression(parameters)).AppendSpace();
			return this;
		}

		public Statement PARAMETERS(IDictionary<string, object> args)
		{
			List<Expression> list = new List<Expression>();
			foreach (var kvp in args)
			{
				object value = kvp.Value;
				list.Add(new Expression(new ParameterName(kvp.Key)).LET(value));
			}

			return PARAMETERS(list);
		}

		public Statement PARAMETERS(object args)
		{
			var properties = args.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(args));
			return PARAMETERS(properties);
		}


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


using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Text
{
    public class Statement
    {
        private readonly List<string> lines = new List<string>();
        protected bool compound = false;

        public static readonly Statement BREAK = new Statement().AppendLine("BREAK");
        public static readonly Statement CONTINUE = new Statement().AppendLine("CONTINUE");
        public static readonly Statement RETURN = new Statement().AppendLine("RETURN");

        public Statement()
        {
        }

        private Statement AppendLine(string line)
        {
            lines.Add(line);
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
            AppendLine($"IF {condition} {then}");
            return this;
        }

        public Statement IF(Expression condition, Statement then, Statement _else)
        {
            AppendLine($"IF {condition} {then} ELSE {_else}");
            return this;
        }

        public Statement WHILE(Expression condition, Statement loop)
        {
            AppendLine($"WHILE {condition} {loop}");
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
            return AppendLine($"SET {key} {value}");
        }

        /// <summary>
        /// SET @VAR = 12
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Statement SET(VariableName name, Expression value)
        {
            AppendLine($"SET @{name} = {value}");
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

        public static implicit operator Statement(SqlBuilder sql)
        {
            return new Statement().AppendLine(sql.Script);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string line in lines)
                builder.Append(line);

            return builder.ToString().Trim();
        }
    }
}

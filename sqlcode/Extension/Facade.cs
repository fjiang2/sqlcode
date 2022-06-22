using System;
using System.Collections.Generic;
using System.Data;

namespace Sys.Data
{
    static class Facade
    {
        public static string ToScript(this SqlValue value, DbAgentStyle style)
        {
            object obj = value.Value;
            switch (style)
            {
                case DbAgentStyle.SqlServer:
                    return new ValueOfScript(obj).ToScript();

                case DbAgentStyle.SQLite:
                    return new ValueOfSQLite(obj).ToScript();

                case DbAgentStyle.SqlCe:
                    return new ValueOfSqlCe(obj).ToScript();

                case DbAgentStyle.Postgres:
                    return new ValueOfPostgre(obj).ToScript();
            }

            throw new NotImplementedException($"cannot find agent {style}");
        }

        public static NameOfScript CreateNameOfScript(this DbAgentStyle style, string name)
        {
            switch (style)
            {
                case DbAgentStyle.SqlServer:
                    return new NameOfSqlServer(name);

                case DbAgentStyle.SQLite:
                case DbAgentStyle.SqlCe:
                    return new NameOfScript(name);

                case DbAgentStyle.Postgres:
                    return new NameOfPostgres(name);

            }

            throw new NotImplementedException($"cannot find agent {style}");
        }

        public static string ToColumnName(this string name, DbAgentStyle style)
        {
            var naming = style.CreateNameOfScript(name);
            return naming.FormalName();
        }

        public static string ToTableName(this string name, DbAgentStyle style)
        {
            //If it is a single letter
            if (name.Length == 1 && char.IsLetter(name[0]))
                return name;

            var naming = style.CreateNameOfScript(name);
            return naming.FormalName();
        }

        public static string ToSchemaName(this string name, DbAgentStyle style)
        {
            var naming = style.CreateNameOfScript(name);
            return naming.SchemaName();
        }

        public static string ToDatabaseName(this string name, DbAgentStyle style)
        {
            var naming = style.CreateNameOfScript(name);
            return naming.FormalName();
        }

        public static string ToParameterName(this string name, DbAgentStyle style)
        {
            var naming = style.CreateNameOfScript(name);
            return naming.ParameterName();
        }
    }
}

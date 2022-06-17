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

            }

            throw new NotImplementedException($"cannot find agent {style}");
        }

        public static NameOfScript CreateNameOfScript(this DbAgentStyle style, string name)
        {
            switch (style)
            {
                case DbAgentStyle.SqlServer:
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
            return naming.ColumnName();
        }

        public static string ToTableName(this string name, DbAgentStyle style)
        {
            var naming = style.CreateNameOfScript(name);
            return naming.TableName();
        }

        public static string ToParameterName(this string name, DbAgentStyle style)
        {
            var naming = style.CreateNameOfScript(name);
            return naming.ParameterName();
        }
    }
}

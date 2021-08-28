using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Text
{
    static class SqlExtension
    {
     
        public static Expression SqlType(this Type type)
        {
            return type.ToSqlType().AsVariable();
        }

        public static Expression DateIntervalType(this DateInterval interval)
        {
            return interval.ToString().AsVariable();
        }

        public static string ToSqlType(this Type type)
        {
            if (type == typeof(bool))
                return "BIT";

            else if (type == typeof(byte))
                return "TINYINT";
            else if (type == typeof(sbyte))
                return "SMALLINT";

            else if (type == typeof(short))
                return "SMALLINT";
            else if (type == typeof(ushort))
                return "INT";

            else if (type == typeof(int))
                return "INT";
            else if (type == typeof(uint))
                return "BIGINT";

            else if (type == typeof(long))
                return "BIGINT";
            else if (type == typeof(ulong))
                return "DECIMAL";

            else if (type == typeof(float))
                return "REAL";
            else if (type == typeof(double))
                return "FLOAT";

            else if (type == typeof(decimal))
                return "DECIMAL";

            else if (type == typeof(string))
                return "NVARCHAR";

            else if (type == typeof(DateTime))
                return "DATETIME";
            else if (type == typeof(DateTimeOffset))
                return "DATETIMEOFFSET";
            else if (type == typeof(TimeSpan))
                return "TIME";

            else if (type == typeof(byte[]))
                return "BINARY";

            else if (type == typeof(Guid))
                return "UniqueIdentifier";

            if (type.IsEnum)
                return "INT";

            throw new Exception($"Type {type} cannot be converted into SqlDbType");
        }

        public static StringType GetStringType(string text)
        {
            if (text.StartsWith("@"))
                return StringType.Parameter;

            string[] L1 = text.Split(new char[] { '.' });
            if (L1.Length == 0)
                return StringType.String;


            return StringType.String;
        }
    }
}

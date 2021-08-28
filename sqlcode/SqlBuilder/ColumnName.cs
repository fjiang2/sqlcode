using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Text
{
    class ColumnName
    {
        private readonly string tableName;
        private readonly string name;

        public ColumnName(string columnName)
        {
            this.name = columnName;
        }

        public ColumnName(string tableName, string columnName)
        {
            this.tableName = tableName;
            this.name = columnName;
        }

        public ColumnName(ITableName tableName, string columnName)
        {
            this.tableName = tableName.FullName;
            this.name = columnName;
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            if (tableName != null)
                text.Append(tableName).Append(".");

            if (name == "*" || string.IsNullOrEmpty(name))
                text.Append("*");
            else
                text.Append($"[{name}]");

            return text.ToString();
        }

        public static explicit operator Expression(ColumnName columnName)
        {
            return new Expression(columnName);
        }

    }
}

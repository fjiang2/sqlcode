using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Text
{
    class ColumnName : IQueryScript
    {
        private readonly ITableName tableName;
        private readonly string name;

        public ColumnName(string columnName)
        {
            this.name = columnName;
        }

        
        public ColumnName(ITableName tableName, string columnName)
        {
            this.tableName = tableName;
            this.name = columnName;
        }

        public string ToScript(DbAgentStyle style)
        {
            StringBuilder text = new StringBuilder();
            if (tableName != null)
                text.Append(tableName.ToScript(style)).Append(".");

            if (name == "*" || string.IsNullOrEmpty(name))
                text.Append("*");
            else
                text.Append(name.ToColumnName(style));

            return text.ToString();
        }

        public override string ToString()
		{
            return ToScript(DbAgentOption.DefaultStyle);
		}

        public static explicit operator Expression(ColumnName columnName)
        {
            return new Expression(columnName);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.Data
{
    public class SqlTemplate
    {
        private readonly string TableName;
        private SqlTemplateFormat format = SqlTemplateFormat.SingleLine;
        private string NewLine = string.Empty;

        public DbAgentStyle Style { get; set; } = DbAgentOption.DefaultStyle;

        public SqlTemplate(string formalName)
        {
            this.TableName = formalName;
        }


        internal SqlTemplateFormat Format
        {
            get
            {
                return format;
            }
            set
            {
                format = value;
                switch (format)
                {
                    case SqlTemplateFormat.Indent:
                        NewLine = Environment.NewLine + "  ";
                        break;

                    default:
                        NewLine = string.Empty;
                        break;
                }
            }
        }

        private string IfNotExists(string where) => $"IF NOT EXISTS(SELECT * FROM {TableName} WHERE {where})";
        private string IfExists(string where) => $"IF EXISTS(SELECT * FROM {TableName} WHERE {where})";

        public string IfNotExistsInsert(string where, string insert)
            => $"{IfNotExists(where)} {NewLine}{insert}";

        public string IfExistsUpdate(string where, string update)
            => $"{IfExists(where)} {NewLine}{update}";
        public string IfExistsUpdateElseInsert(string where, string update, string insert)
            => $"{IfExists(where)} {NewLine}{update} {NewLine}ELSE {NewLine}{insert}";

        public string Select(string select)
            => $"SELECT {select} {NewLine}FROM {TableName}";
        public string Select(string select, string where)
            => $"{Select(select)} {NewLine}WHERE {where}";

        public string Update(string set)
            => $"UPDATE {TableName} {NewLine}SET {set}";
        public string Update(string set, string where)
            => $"{Update(set)} {NewLine}WHERE {where}";

        public string Insert(string values)
            => Insert("INSERT", string.Empty, values);
        public string Insert(string columns, string values)
            => Insert("INSERT", columns, values);
        public string InsertOrIgnore(string columns, string values)
            => Insert("INSERT OR IGNORE", columns, values);
        public string InsertOrReplace(string columns, string values)
            => Insert("INSERT OR REPLACE", columns, values);
        private string Insert(string cmd, string columns, string values)
        {
            string _columns = string.Empty;
            if (!string.IsNullOrWhiteSpace(columns))
                _columns = $"({columns})";

            return $"{cmd} INTO {TableName}{_columns} {NewLine}VALUES({values})";
        }

        internal static string SetIdentityOutParameter(string parameterName)
            => $"SET {parameterName}=@@IDENTITY";

        public string Delete()
            => $"DELETE FROM {TableName}";
        public string Delete(string where)
            => $"{Delete()} {NewLine}WHERE {where}";


        public string AddPrimaryKey(string primaryKey)
            => $"ALTER TABLE {TableName} ADD PRIMARY KEY ({primaryKey})";
        public string DropPrimaryKey(string constraintName)
            => $"ALTER TABLE {TableName} DROP CONSTRAINT ({constraintName})";

        public string DropForeignKey(string constraintName)
            => $"ALTER TABLE {TableName} DROP CONSTRAINT ({constraintName})";

        public string AddColumn(string column)
            => $"ALTER TABLE {TableName} ADD {column}";

        public string AddColumn(string column, object defaultValue)
        {
            string value = new SqlValue(defaultValue).ToScript(Style);
            return $"ALTER TABLE {TableName} ADD {column} DEFAULT({value})";
        }

        public string AlterColumn(string column)
            => $"ALTER TABLE {TableName} ALTER COLUMN {column}";
        public string DropColumn(string column)
            => $"ALTER TABLE {TableName} DROP COLUMN {column}";


        public string DropTable(bool ifExists)
        {
            if (ifExists)
            {
                return new StringBuilder()
                .AppendLine($"IF OBJECT_ID('{TableName}') IS NOT NULL")
                .AppendLine($"  DROP TABLE {TableName}")
                .ToString();
            }

            return $"DROP TABLE {TableName}";
        }

        public override string ToString()
        {
            return TableName;
        }
    }
}

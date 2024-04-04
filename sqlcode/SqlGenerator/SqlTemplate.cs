using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.Data
{
    class SqlTemplate
    {
        private readonly string TableName;
        private readonly string NewLine = string.Empty;

        public SqlTemplate(string formalName, SqlTemplateFormat format)
        {
            this.TableName = formalName;
            if (format == SqlTemplateFormat.Indent)
            {
                NewLine = Environment.NewLine + "  ";
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

        public string Insert(string cmd, string columns, string values)
        {
            string _columns = string.Empty;
            if (!string.IsNullOrWhiteSpace(columns))
                _columns = $"({columns})";

            if (cmd == null)
                cmd = "INSERT";

            return $"{cmd} INTO {TableName}{_columns} {NewLine}VALUES({values})";
        }

        internal static string SetIdentityOutParameter(string parameterName)
            => $"SET {parameterName}=@@IDENTITY";

        public string Delete()
            => $"DELETE FROM {TableName}";
        public string Delete(string where)
            => $"{Delete()} {NewLine}WHERE {where}";

        public override string ToString()
        {
            return TableName;
        }
    }
}

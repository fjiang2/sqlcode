using System;

namespace Sys.Data
{
    class SqlGeneratorOfSQLite : SqlGenerator
    {
        internal SqlGeneratorOfSQLite(string formalName)
            : base(formalName)
        {
        }

        protected override string IfNotExistsInsert(string where)
        {
            return Insert("INSERT OR IGNORE");
        }

        protected override string IfExistsUpdateElseInsert(string where)
        {
            string insert = Insert("INSERT OR IGNORE");
            string update = Update();
            if (Format == SqlTemplateFormat.Indent)
                return $"{insert};{Environment.NewLine}{update}";
            else
                return $"{insert};{update}";
        }
    }
}

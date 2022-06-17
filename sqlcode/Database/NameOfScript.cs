using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data
{
    public class NameOfScript
    {
        private readonly string name;

        public NameOfScript(string name)
        {
            this.name = name;
        }

        public virtual string LeftBracket => "[";
        public virtual string RightBracket => "]";

        public virtual string DefaultSchemaName => "dbo";

        public string ColumnName()
        {
            if (name.StartsWith(LeftBracket) && name.EndsWith(RightBracket))
                return name;

            return $"{LeftBracket}{name}{RightBracket}";
        }

        public virtual string TableName()
        {
            if (name.StartsWith(LeftBracket) && name.EndsWith(RightBracket))
                return name;

            return $"{LeftBracket}{name}{RightBracket}";
        }

        public virtual string ParameterName()
        {
            return "@" + name;
        }

    }
}

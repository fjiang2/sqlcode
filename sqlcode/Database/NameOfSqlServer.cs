using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data
{
    class NameOfSqlServer : NameOfScript
    {
        public NameOfSqlServer(string name)
            :base(name)
        {
        }

        protected override string LeftBracket => "[";
        protected override string RightBracket => "]";

        public override string DefaultSchemaName => "dbo";
    }
}

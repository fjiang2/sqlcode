using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data
{
    public class NameOfPostgres : NameOfScript
    {
        public NameOfPostgres(string name)
            :base(name)
        {
        }

        protected override string LeftBracket => "\"";
        protected override string RightBracket => "\"";

        public override string DefaultSchemaName => "public";
    }
}

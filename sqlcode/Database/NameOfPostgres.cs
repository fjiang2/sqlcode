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

        public override string LeftBracket => "\"";
        public override string RightBracket => "\"";
        public override string DefaultSchemaName => "public";
    }
}

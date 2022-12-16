using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Text
{
    class ParameterName : IQueryScript
    {
        private readonly string name;

        public ParameterName(string name)
        {
            this.name = name;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var x = obj as ParameterName;
            if (x == null)
                return false;

            return this.name == x.name;
        }

        public string ToScript(DbAgentStyle style)
        {
            return name.ToParameterName(style);
        }

        public override string ToString()
        {
            return ToScript(DbAgentOption.DefaultStyle);
        }
    }
}

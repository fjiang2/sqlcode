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

        public virtual string FormalName()
        {
            if (name.StartsWith("[") && name.EndsWith("]"))
                return name;

            return $"[{name}]";
        }
    }
}

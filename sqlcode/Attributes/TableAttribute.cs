using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Attributes
{
    public class TableAttribute : Attribute
    {
        public string Name { get; }
        public string SchemaName { get; set; }

        public TableAttribute(string name)
        {
            this.Name = name;
        }
    }

}

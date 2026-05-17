using System;

namespace Sys.Data.Attributes
{
    public class IdentityKeyAttribute : Attribute, IColumnsAttribute
    {
        public string[] Columns { get; }

        public IdentityKeyAttribute(params string[] columns)
        {
            this.Columns = columns;
        }
    }

}

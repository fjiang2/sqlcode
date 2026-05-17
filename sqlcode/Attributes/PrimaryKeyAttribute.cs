using System;

namespace Sys.Data.Attributes
{
    public class PrimaryKeyAttribute : Attribute, IColumnsAttribute
    {
        public string[] Columns { get; }

        public PrimaryKeyAttribute(params string[] columns)
        {
            this.Columns = columns;
        }
    }

}

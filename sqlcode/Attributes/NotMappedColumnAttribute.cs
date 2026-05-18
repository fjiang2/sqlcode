using System;

namespace Sys.Data.Attributes
{
    public class NotMappedColumnAttribute : Attribute, IColumnsAttribute
    {
        public string[] Columns { get; }

        public NotMappedColumnAttribute(params string[] columns)
        {
            this.Columns = columns;
        }
    }

}

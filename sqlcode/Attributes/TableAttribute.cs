using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Attributes
{
    public class TableAttribute : Attribute
    {
        public string Name { get; }
        public string SchemaName { get; set; } = "dbo";

        public TableAttribute(string name)
        {
            this.Name = name;
        }

    }

    public interface IColumnsAttribute
    {
        string[] Columns { get; }
    }

    public class PrimaryKeysAttribute : Attribute, IColumnsAttribute
    {
        public string[] Columns { get; }

        public PrimaryKeysAttribute(params string[] primaryKeys)
        {
            this.Columns = primaryKeys;
        }
    }

    public class IdentityKeysAttribute : Attribute, IColumnsAttribute
    {
        public string[] Columns { get; }

        public IdentityKeysAttribute(params string[] identityKeys)
        {
            this.Columns = identityKeys;
        }
    }

    public class NotMappedColumnsAttribute : Attribute, IColumnsAttribute
    {
        public string[] Columns { get; }

        public NotMappedColumnsAttribute(params string[] columns)
        {
            this.Columns = columns;
        }
    }

}

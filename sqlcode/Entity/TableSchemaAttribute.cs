using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Entity
{
    public class TableSchemaAttribute : Attribute
    {
        public string TableName { get; }
        public string SchemaName { get; set; } = "dbo";

        public string[] PrimaryKeys { get; set; } = new string[0];
        public string[] IdentityKeys { get; set; } = new string[0];

        public TableSchemaAttribute(string name)
        {
            this.TableName = name;
        }

    }
}

﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.Data.Entity
{
    class TableSchema : ITableSchema
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string[] PrimaryKeys { get; set; }
        public string[] IdentityKeys { get; set; }
        public IConstraint[] Constraints { get; set; }

        public override string ToString()
        {
            return $"{SchemaName}.{TableName}, pk=({string.Join(",", PrimaryKeys)})";
        }
    }

}

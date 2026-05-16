using Sys.Data.Entity;

namespace Sys.Data.EfCore
{
    class TableSchema : ITableSchema
    {
        public string? SchemaName { get; set; }
        public string? TableName { get; set; }
        public string[]? PrimaryKeys { get; set; }
        public string[]? IdentityKeys { get; set; }
        public IConstraint[]? Constraints { get; set; }
    }
}

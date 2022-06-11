namespace Sys.Data
{
    public class SqlValueChange
    {
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DataColumnState ColumnState { get; set; }

        public SqlValueChange()
        { 
        }

        public override string ToString()
        {
            switch (ColumnState)
            {
                case DataColumnState.Added:
                    return $"{ColumnName} inserted: -> {NewValue}";

                case DataColumnState.Deleted:
                    return $"{ColumnName} deleted: {OldValue} ->";

                case DataColumnState.Modified:
                    return $"{ColumnName} modified: {OldValue} -> {NewValue}";

                case DataColumnState.Unchanged:
                    return $"{ColumnName} unchanged: {OldValue} == {NewValue}";
            }

            return ColumnName;
        }
    }
}

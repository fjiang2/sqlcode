using System.Collections.Generic;

namespace Sys.Data
{
    class SqlColumnJourney
    {
        private readonly List<SqlValueChange> list = new List<SqlValueChange>();
        
        public SqlColumnJourney()
        {
        }

        public List<SqlValueChange> Changes => list;

        public void Clear()
        {
            list.Clear();
        }

        public void Add(SqlColumn column, SqlValue newValue)
        {
            var item = new SqlValueChange
            {
                ColumnName = column.Name,
                ColumnState = DataColumnState.Added,
                NewValue = newValue.ToString(),
            };

            list.Add(item);
        }

        public void Modify(SqlColumn column, SqlValue oldValue, SqlValue newValue)
        {
            var item = new SqlValueChange
            {
                ColumnName = column.Name,
                ColumnState = DataColumnState.Modified,
                OldValue = oldValue.ToString(),
                NewValue = newValue.ToString(),
            };

            list.Add(item);
        }

        public void Retain(SqlColumn column, SqlValue oldValue, SqlValue newValue)
        {
            var item = new SqlValueChange
            {
                ColumnName = column.Name,
                ColumnState = DataColumnState.Unchanged,
                OldValue = oldValue.ToString(),
                NewValue = newValue.ToString(),
            };

            list.Add(item);
        }

        public void Delete(SqlColumn column, SqlValue oldValue)
        {
            var item = new SqlValueChange
            {
                ColumnName = column.Name,
                ColumnState = DataColumnState.Deleted,
                OldValue = oldValue.ToString(),
            };

            list.Add(item);
        }

        public override string ToString()
        {
            return list.ToString();
        }
    }
}

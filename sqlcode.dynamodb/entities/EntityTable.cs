﻿using System.Collections.Generic;
using System.Data;

namespace sqlcode.dynamodb.entities
{
    public class EntityTable : List<EntityRow>
    {
        public EntityTable()
        {
        }

        public DataTable ToDataTable()
        {
            var dt = new DataTable();
            foreach (EntityRow row in this)
            {
                foreach (KeyValuePair<string, EntityValue> col in row)
                {
                    string columnName = col.Key;
                    if (!dt.Columns.Contains(columnName))
                    {
                        dt.Columns.Add(new DataColumn(columnName, col.Value.Type));
                    }
                }
            }

            foreach (EntityRow row in this)
            {
                var newRow = dt.NewRow();
                foreach (var col in row)
                {
                    newRow[col.Key] = col.Value.Value;
                }

                dt.Rows.Add(newRow);
            }

            dt.AcceptChanges();
            return dt;
        }
    }
}
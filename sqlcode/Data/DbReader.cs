using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Threading;

namespace Sys.Data
{
    public class DbReader
    {
        private readonly DbDataReader reader;

        public DbReader(DbDataReader reader)
        {
            this.reader = reader;
        }


        private DataRow ReadRow(DataTable table)
        {
            DataRow row = table.NewRow();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[i] = reader.GetValue(i);
            }

            return row;
        }

        public int ReadTable(DataTable table, int startRecord, int maxRecords)
        {
            CreateBlankTable(table, reader);

            int index = -1;
            int count = 0;
            while (reader.Read())
            {
                index++;
                if (index < startRecord)
                    continue;

                var row = ReadRow(table);
                table.Rows.Add(row);

                if (maxRecords > 0 && ++count >= maxRecords)
                    break;
            }

            table.AcceptChanges();
            return count;
        }

        public void ReadDataSet(DataSet ds, int startRecord, int maxRecords)
        {
            while (reader.HasRows)
            {
                var dt = new DataTable();
                ReadTable(dt, startRecord, maxRecords);
                ds.Tables.Add(dt);
                reader.NextResult();
            }
        }

        private static void CreateBlankTable(DataTable table, DbDataReader reader)
        {
            if (table != null)
            {
                table.CaseSensitive = true;
                table.Columns.Clear();
                table.Clear();
                table.AcceptChanges();
            }
            else
            {
                table = new DataTable { CaseSensitive = true };
            }

            for (int i = 0; i < reader.FieldCount; i++)
            {
                DataColumn column = new DataColumn(reader.GetName(i), reader.GetFieldType(i));
                table.Columns.Add(column);
            }

            table.AcceptChanges();
        }
    }
}

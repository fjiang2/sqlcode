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

        public int StartRecord { get; set; } = 0;
        public int MaxRecords { get; set; } = 0;

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

        public int ReadTable(DataTable table)
        {
            CreateBlankTable(table, reader);
            return ReadRows(table);
        }

        private int ReadRows(DataTable table)
        {
            int index = -1;
            int count = 0;
            while (reader.Read())
            {
                index++;
                if (index < StartRecord)
                    continue;

                var row = ReadRow(table);
                table.Rows.Add(row);
                count++;

                if (MaxRecords > 0 && count >= MaxRecords)
                    break;
            }

            table.AcceptChanges();
            return count;
        }

        public int ReadDataSet(DataSet ds)
        {
            int count = 0;

            //read empty table
            var dt = new DataTable();
            CreateBlankTable(dt, reader);
            ds.Tables.Add(dt);

            while (reader.HasRows)
            {
                count += ReadRows(dt);
                if(reader.NextResult())
                {
                    //read next empty table
                    dt = new DataTable();
                    CreateBlankTable(dt, reader);
                    ds.Tables.Add(dt);
                }
            }

            return count;
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

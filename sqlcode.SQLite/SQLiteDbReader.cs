using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Sys.Data.SQLite
{
    internal class SQLiteDbReader : DbReader
    {
        public SQLiteDbReader(SQLiteDataReader reader)
            : base(reader)
        {
        }

        protected override object GetValue(int ordinal)
        {
            object value = reader.GetValue(ordinal);

            //SQLite has issue in DateTime
            if (value is DateTime time1)
            {
                string text = reader.GetString(ordinal);
                if (DateTime.TryParse(text, out var time2) && time1 < time2)
                    value = time2;
            }
            else if (value is DateTimeOffset offset1)
            {
                string text = reader.GetString(ordinal);
                if (DateTimeOffset.TryParse(text, out var offset2) && offset1 < offset2)
                    value = offset2;
            }

            return value;
        }
    }
}

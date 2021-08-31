using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Sys.Data
{

    public interface IDbFill
    {
        DataSet FillDataSet();
        DataTable FillDataTable(int table);
        DataRow FillDataRow(int row, int table);
        List<T> FillDataColumn<T>(int column, int table);
        List<T> FillDataColumn<T>(string columnName, int table);
        T FillObject<T>(int column, int row, int table);
        T FillObject<T>(string column, int row, int table);
    }
}

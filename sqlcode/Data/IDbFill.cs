using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Sys.Data
{

    public interface IDbFill
    {
        DataSet FillDataSet();
        DataTable FillDataTable();
        DataRow FillDataRow();
        List<T> FillDataColumn<T>(int column);
        List<T> FillDataColumn<T>(string columnName);
        object FillObject();
        T FillObject<T>();
    }
}

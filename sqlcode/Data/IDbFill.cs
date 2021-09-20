using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Sys.Data
{
    /// <summary>
    /// Fill data-set, data-table, data-row or data-column
    /// </summary>
    public interface IDbFill
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DataSet FillDataSet();

        /// <summary>
        /// Return the first table
        /// </summary>
        /// <returns></returns>
        DataTable FillDataTable();

        /// <summary>
        /// Return the first row of first table
        /// </summary>
        /// <returns></returns>
        DataRow FillDataRow();

        /// <summary>
        /// Return column list from the first table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        List<T> FillDataColumn<T>(int column);

        /// <summary>
        /// Return column list from the first table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <returns></returns>
        List<T> FillDataColumn<T>(string columnName);

        /// <summary>
        /// Return the first column from first row in first table
        /// </summary>
        /// <returns></returns>
        object FillObject();

        /// <summary>
        /// Return the first column from first row in first table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T FillObject<T>();
    }
}

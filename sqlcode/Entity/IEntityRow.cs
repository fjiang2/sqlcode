using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sys.Data.Entity
{
    public interface IEntityRow
    {
        void FillObject(DataRow row);
        IDictionary<string, object> ToDictionary();

        /// <summary>
        /// Used on updating data table.
        /// </summary>
        /// <param name="row"></param>
        void UpdateRow(DataRow row);
    }
}

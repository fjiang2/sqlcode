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
    }
}

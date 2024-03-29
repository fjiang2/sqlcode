﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.Data.Entity
{
    public class RowEvent
    {
        public string TypeName { get; set; }

        public RowOperation Operation { get; set; }

        public IDictionary<string, object> Row { get; set; }

        public override string ToString()
        {
            return $"{TypeName} {Operation} : Count={Row.Count}";
        }
    }
}

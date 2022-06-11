using System;

namespace Sys.Data
{
    [Flags]
    public enum DataColumnState
    {
        Detached = 1,
        Unchanged = 2,
        Added = 4,
        Deleted = 8,
        Modified = 16
    }
}

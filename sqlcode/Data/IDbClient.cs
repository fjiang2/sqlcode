﻿using Sys.Data;
using Sys.Data.Entity;

namespace Sys.Data
{
    public interface IDbClient
    {
        IDbAgent Agent { get; }
        IDbContext Context { get; }
        IDbQuery Query { get; }

        void SetDefaultAgent();
    }
}
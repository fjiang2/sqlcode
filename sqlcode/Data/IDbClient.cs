using Sys.Data;
using Sys.Data.Entity;

namespace Sys.Data
{
    public interface IDbClient
    {
        IDbAgent Agent { get; }
        IDbContext Context { get; }
        DataQuery Query { get; }

        void SetDefaultAgent();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Sys.Data.Entity
{
    public static class EntityExtension
    {
        public static TEntity CreateEntity<TEntity>(this DataRow row) where TEntity : IEntityRow, new()
        {
            var obj = new TEntity();
            obj.FillObject(row);
            return obj;
        }

        public static List<TEntity> ToList<TEntity>(this DataTable dt) where TEntity : IEntityRow, new()
        {
            List<TEntity> list = new List<TEntity>();
            foreach (DataRow row in dt.Rows)
            {
                var obj = new TEntity();
                obj.FillObject(row);
                list.Add(obj);
            }
            return list;
        }
    }
}

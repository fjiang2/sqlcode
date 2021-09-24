using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.Data.Entity
{
    /// <summary>
    /// Column constraint implementation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Constraint<TEntity> : IConstraint
    {
        /// <summary>
        /// Constraint name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The column in current table
        /// </summary>
        public string ThisKey { get; set; }

        /// <summary>
        /// The column in other table
        /// </summary>
        public string OtherKey { get; set; }

        /// <summary>
        /// Is it foreign key
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// one-to-many
        /// </summary>
        public bool OneToMany { get; set; }

        /// <summary>
        /// Type of entity
        /// </summary>
        public Type OtherType => typeof(TEntity);
    }
}

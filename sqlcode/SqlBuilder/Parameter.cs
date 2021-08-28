using System;
using System.Data;

namespace Sys.Data.Text
{
    public class Parameter
    {
		public ParameterName Name { get; set; }
		public Type ParameterType { get; set; }
		public ParameterDirection Direction { get; set; }

		public Parameter()
        {

        }

		public override string ToString()
		{
			return $"{Name} {ParameterType.ToSqlType()}";
		}
	}
}

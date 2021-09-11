using System.Collections.Generic;
using System.Data;

namespace Sys.Data
{
	public interface IParameterFactory
	{
		List<IDataParameter> Create();
		void Update(IEnumerable<IDataParameter> result);
	}
}
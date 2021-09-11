using System.Collections.Generic;
using System.Data;

namespace Sys.Data
{
	public interface IParameterFactory
	{
		List<IDataParameter> CreateParameters();
		void UpdateResult(IEnumerable<IDataParameter> result);
	}
}
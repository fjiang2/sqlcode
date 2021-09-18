using System.Collections.Generic;
using System.Data;

namespace Sys.Data
{
	public interface IParameterFacet
	{

		/// <summary>
		/// Create parameters
		/// </summary>
		/// <returns></returns>
		List<IDataParameter> CreateParameters();

		/// <summary>
		/// Update values of out/ref parameters from result
		/// </summary>
		/// <param name="result"></param>
		void UpdateResult(IEnumerable<IDataParameter> result);
	}
}
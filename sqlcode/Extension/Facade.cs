using System;
using System.Collections.Generic;
using System.Data;

namespace Sys.Data
{
	static class Facade
	{
		public static string ToScript(this SqlValue value, DbAgentStyle style)
		{
			object obj = value.Value;
			switch (style)
			{
				case DbAgentStyle.SqlServer:
					return new ValueOfScript(obj).ToScript();

				case DbAgentStyle.SQLite:
					return new ValueOfSQLite(obj).ToScript();

				case DbAgentStyle.SqlCe:
					return new ValueOfSqlCe(obj).ToScript();

			}

			throw new NotImplementedException($"cannot find agent {style}");
		}

		public static string ToScript(this SqlColumn column, DbAgentStyle style)
        {
			string name = column.Name;
			switch (style)
			{
				case DbAgentStyle.SqlServer:
				case DbAgentStyle.SQLite:
				case DbAgentStyle.SqlCe:
					return new NameOfScript(name).FormalName();

			}

			throw new NotImplementedException($"cannot find agent {style}");
		}
	}
}

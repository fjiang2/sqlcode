using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Text
{
	public class TYPE : IQueryScript
	{
		string type;
		private TYPE(string type)
		{
			this.type = type;
		}

		public string ToScript(DbAgentStyle style)
		{
			return type;
		}

		public override string ToString()
		{
			return ToScript(DbAgentOption.DefaultStyle);
		}

		public static explicit operator Expression(TYPE type)
		{
			return new Expression(type);
		}

		public static TYPE TINYINT => new TYPE("TINYINT");
		public static TYPE SMALLINT => new TYPE("SMALLINT");
		public static TYPE INT => new TYPE("INT");
		public static TYPE BIGINT => new TYPE("BIGINT");

		public static TYPE FLOAT => new TYPE("FLOAT");
		public static TYPE REAL => new TYPE("REAL");

		public static TYPE BIT => new TYPE("BIT");
		public static TYPE BOOL => new TYPE("BOOL");

		public static TYPE MONEY => new TYPE("MONEY");
		public static TYPE SMALLMONEY => new TYPE("SMALLMONEY");

		public static TYPE SMALLDATETIME => new TYPE("SMALLDATETIME");
		public static TYPE DATETIME => new TYPE("DATETIME");
		public static TYPE DATE => new TYPE("DATE");
		public static TYPE TIME => new TYPE("TIME");
		public static TYPE TIMESTAMP => new TYPE("TIMESTAMP");
		public static TYPE DATETIMEOFFSET => new TYPE("DATETIMEOFFSET");

		public static TYPE DECIMAL(int size, int precision) => new TYPE($"DECIMAL({size}, {precision})");
		public static TYPE NUMERIC(int size, int precision) => new TYPE($"NUMERIC({size}, {precision})");

		public static TYPE TEXT(int length) => new TYPE($"TEXT({length})");
		public static TYPE NTEXT(int length) => new TYPE($"NTEXT({length})");
		public static TYPE CHAR(int length) => new TYPE($"CHAR({length})");
		public static TYPE NCHAR(int length) => new TYPE($"NCHAR({length})");
		public static TYPE VARCHAR(int length) => new TYPE($"VARCHAR({length})");
		public static TYPE NVARCHAR(int length) => new TYPE($"NVARCHAR({length})");

		public static TYPE BINARY(int length) => new TYPE($"BINARY({length})");
		public static TYPE VARBINARY(int length) => new TYPE($"VARBINARY({length})");
		public static TYPE IMAGE(int length) => new TYPE($"IMAGE({length})");

		public static TYPE XML => new TYPE("XML");
		public static TYPE TABLE => new TYPE("TABLE");
		public static TYPE UNIQUEIDENTIFIER => new TYPE("UNIQUEIDENTIFIER");

	}
}

//--------------------------------------------------------------------------------------------------//
//                                                                                                  //
//        DPO(Data Persistent Object)                                                               //
//                                                                                                  //
//          Copyright(c) Datum Connect Inc.                                                         //
//                                                                                                  //
// This source code is subject to terms and conditions of the Datum Connect Software License. A     //
// copy of the license can be found in the License.html file at the root of this distribution. If   //
// you cannot locate the  Datum Connect Software License, please send an email to                   //
// datconn@gmail.com. By using this source code in any fashion, you are agreeing to be bound        //
// by the terms of the Datum Connect Software License.                                              //
//                                                                                                  //
// You must not remove this notice, or any other, from this software.                               //
//                                                                                                  //
//                                                                                                  //
//--------------------------------------------------------------------------------------------------//
using System;

namespace Sys.Data.Text
{
    class TableName : IQueryScript, ITableName
    {
        public const string empty = "";

        private readonly string databaseName;
        private readonly string schemaName;
        private readonly string tableName;

        public TableName(string fullTableName)
        {
            //tableName may have format like [db.dbo.tableName], [db..tableName], or [tableName]
            string[] t = fullTableName.Split(new char[] { '.' });

            this.databaseName = empty;
            this.schemaName = empty;
            this.tableName = empty;

            if (t.Length > 2)
            {
                this.databaseName = t[0];
                this.schemaName = t[1];
                this.tableName = t[2];
            }
            else if (t.Length > 1)
            {
                this.schemaName = t[0];
                this.tableName = t[1];
            }
            else
                this.tableName = fullTableName;
        }

        public TableName(string databaseName, string schemaName, string tableName)
        {
            this.databaseName = databaseName ?? string.Empty;
            this.schemaName = schemaName ?? string.Empty;
            this.tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        public TableName(string schemaName, string tableName)
            : this(string.Empty, schemaName, tableName)
        {
        }

        public string SchemaName => this.schemaName;

        public string DatabaseName => this.databaseName;

        public string Name => this.tableName;

        public string FormalName(DbAgentStyle style)
        {
            string _tableName = tableName.ToTableName(style);
            string _schemaName = schemaName.ToSchemaName(style);

            if (_schemaName != empty)
                return $"{_schemaName}.{_tableName}";
            else
                return _tableName;
        }

        public string ToScript(DbAgentStyle style)
        {
            string _tableName = tableName.ToTableName(style);
            string _schemaName = schemaName.ToSchemaName(style);


            if (this.databaseName != empty)
            {
                string _databaseName = databaseName.ToDatabaseName(style);
                return $"{_databaseName}.{_schemaName}.{_tableName}";
            }
            else if (_schemaName != empty)
            {
                return $"{_schemaName}.{tableName}";
            }
            else
            {
                return _tableName;
            }
        }


        public static implicit operator TableName(string tableName)
        {
            return new TableName(tableName);
        }


        public override string ToString()
        {
            return ToScript(DbAgentOption.DefaultStyle);
        }

    }
}

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
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Reflection;

namespace Sys.Data
{
    class SqlColumn
    {
        private readonly string fieldName;
        private bool saved = true;
        private bool identity = false;

        public Type DataType { get; }
        public bool Primary { get; set; }
        public string Caption { get; set; }

        public SqlColumn(string columnName, Type dbType)
        {
            this.fieldName = columnName;
            this.Caption = columnName;
            this.DataType = dbType;
        }

        public string Name => this.fieldName;

        public bool Saved
        {
            get { return this.saved; }
            set { this.saved = value; }
        }

        public bool Identity
        {
            get { return this.identity; }
            set
            {
                this.identity = value;
                if (this.identity)
                    saved = false;
            }
        }

        public string ToScript(DbAgentStyle style)
        {
            return fieldName.ToColumnName(style);
        }

        public override string ToString()
        {
            return fieldName;
        }
    }
}

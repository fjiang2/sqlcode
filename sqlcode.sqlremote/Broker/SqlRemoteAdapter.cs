﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.IO;

namespace Sys.Data.SqlRemote
{
    internal class SqlRemoteAdapter
    {
        private readonly SqlRemoteRequest request;
        public readonly ISqlRemoteBroker broker;

        public SqlRemoteAdapter(ISqlRemoteBroker broker, SqlRemoteRequest request)
        {
            this.broker = broker;
            this.request = request;
        }

        public int ExecuteNonQuery()
        {
            SqlRemoteResult result = Request();
            return result.Count;
        }

        public void ExecuteTransaction()
        {
            Request();
        }

        public object LoadScalar()
        {
            SqlRemoteResult result = Request();
            return result.Scalar;
        }

        public int LoadDataSet(DataSet dataSet)
        {
            SqlRemoteResult result = Request();

            if (string.IsNullOrEmpty(result.Xml))
                return -1;

            using (var stream = new StringReader(result.Xml))
            {
                dataSet.ReadXml(stream, XmlReadMode.ReadSchema);
            }

            return result.Count;
        }

        public int LoadDataTable(DataTable dataTable)
        {
            SqlRemoteResult result = Request();

            if (string.IsNullOrEmpty(result.Xml))
                return -1;

            using (var stream = new StringReader(result.Xml))
            {
                dataTable.ReadXml(stream);
            }

            return result.Count;
        }

        private SqlRemoteResult Request()
        {
            SqlRemoteResult result = broker.RequesteAsync(request).Result;
            if (result.Error != null)
                throw new Exception(result.Error);

            return result;
        }
    }
}

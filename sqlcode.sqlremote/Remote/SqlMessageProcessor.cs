﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    public class SqlMessageProcessor
    {
        private readonly DbAgent agent;

        public SqlMessageProcessor(DbAgent agent)
        {
            this.agent = agent;
        }

        public SqlResultMessage Process(SqlRequestMessage request)
        {
            SqlUnit unit = new SqlUnit(request.CommandText)
            {
                CommandType = request.CommandType,
                Arguments = request.Parameters,
            };

            var access = agent.Access(unit);

            SqlResultMessage result = new SqlResultMessage();

            string func = request.Function;

            try
            {
                switch (func)
                {
                    case nameof(IDbAccess.ExecuteNonQuery):
                        result.Count = access.ExecuteNonQuery();
                        break;

                    case nameof(IDbAccess.ExecuteTransaction):
                        access.ExecuteTransaction();
                        break;

                    case nameof(IDbAccess.ExecuteScalar):
                        result.Scalar = access.ExecuteScalar();
                        break;

                    case nameof(IDbAccess.FillDataSet):
                    case nameof(IDbAccess.ReadDataSet):
                        DataSet ds = new DataSet();
                        if (func == nameof(IDbAccess.FillDataSet))
                            result.Count = access.FillDataSet(ds);
                        else
                            result.Count = access.ReadDataSet(ds);
                        using (var stream = new StringWriter())
                        {
                            ds.WriteXml(stream, XmlWriteMode.WriteSchema);
                            string xml = stream.ToString();
                            result.Xml = xml;
                        }

                        break;

                    case nameof(IDbAccess.FillDataTable):
                    case nameof(IDbAccess.ReadDataTable):
                        DataTable dt = new DataTable();
                        if (func == nameof(IDbAccess.FillDataTable))
                            result.Count = access.FillDataTable(dt, request.StartRecord, request.MaxRecords);
                        else
                            result.Count = access.ReadDataTable(dt, request.StartRecord, request.MaxRecords);
                        using (var stream = new StringWriter())
                        {
                            dt.WriteXml(stream);
                            string xml = stream.ToString();
                            result.Xml = xml;
                        }
                        break;

                    default:
                        throw new Exception($"invalid SQL request function:{func}");
                }
            }
            catch (Exception ex)
            {
                result.Error = GetAllMessages(ex);
            }

            return result;
        }

        private static string GetAllMessages(Exception exception)
        {
            const string ERROR_ONE_AND_MORE = "One or more errors occurred.";

            StringBuilder builder = new StringBuilder();

            if (exception.Message != ERROR_ONE_AND_MORE)
                builder.AppendLine(exception.Message);

            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                if (innerException.Message != ERROR_ONE_AND_MORE)
                    builder.AppendLine(innerException.Message);

                innerException = innerException.InnerException;
            }

            return builder.ToString();
        }
    }
}

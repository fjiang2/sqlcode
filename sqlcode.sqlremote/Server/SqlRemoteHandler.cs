using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteHandler : ISqlRemoteHandler
    {
        private readonly IDbAgent agent;

        public SqlRemoteHandler(IDbAgent agent)
        {
            this.agent = agent;
        }

        public SqlRemoteResult Execute(SqlRemoteRequest request)
        {
            SqlUnit unit = new SqlUnit(request.CommandText)
            {
                CommandType = request.CommandType,
                Arguments = request.Parameters,
            };

            var access = agent.Access(unit);

            SqlRemoteResult result = new SqlRemoteResult();

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
                        result.Error = $"invalid SQL request function:{func}";
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.GetAllMessages();
            }

            return result;
        }

    }
}

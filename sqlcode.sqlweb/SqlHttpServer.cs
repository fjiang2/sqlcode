using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;

using Sys.Data.SqlRemote;
using Sys.Data.SqlClient;

namespace sqlcode.sqlweb
{
    public class SqlHttpServer
    {
        private readonly HttpListener listener = new HttpListener();
        internal CancellationTokenSource cts { get; }

        public SqlHttpServer(params string[] prefixes)
        {
            this.cts = new CancellationTokenSource();

            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");
            }

            if (prefixes == null || prefixes.Length == 0)
            {
                throw new ArgumentException("URI prefixes are required");
            }

            foreach (var prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
        }


        public void Start()
        {
            listener.Start();

            Console.WriteLine("Webserver running...");
            try
            {
                while (listener.IsListening)
                {
                    HttpListenerContext context = listener.GetContext();
                    try
                    {
                        HttpListenerRequest request = context.Request;
                        Stream input = request.InputStream;
                        string requestString = StreamToString(request.InputStream);

                        HttpListenerResponse response = context.Response;
                        string responseString = Request(requestString);
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                    }
                    catch(Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                    }
                    finally
                    {
                        if (context != null)
                        {
                            context.Response.OutputStream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            listener.Stop();
            listener.Close();
        }

        public static string StreamToString(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public string Request(string json)
        {
            Console.WriteLine($"request:{json}");
            var request = Json.Deserialize<SqlRemoteRequest>(json);

            SqlRemoteResult result = Execute(request);

            json = Json.Serialize(result);
            Console.WriteLine($"response:{json}");

            return json;
        }


        private SqlRemoteResult Execute(SqlRemoteRequest request)
        {
            string connectionString = Program.connectionString;
            var agent = new SqlDbAgent(new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString));

            SqlRemoteHandler handler = new SqlRemoteHandler(agent);
            return handler.Execute(request);
        }
    }
}
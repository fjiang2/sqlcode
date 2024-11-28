﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;

#if NET8_0
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif


using Sys.Data.SqlRemote;
using Sys.Data.SqlClient;
using SqlProxyService.Settings;
using Sys.Data;
using Sys.Data.SQLite;
using System.Data.SQLite;

namespace SqlProxyService.Services
{
    public class SqlHttpWorker : BackgroundService
    {
        private readonly ILogger<SqlHttpWorker> logger;

        private readonly HttpListener listener = new HttpListener();
        private readonly ServerOption serverOption;

        public SqlHttpWorker(ILogger<SqlHttpWorker> logger, ServerOption serverOption)
        {
            this.logger = logger;
            this.serverOption = serverOption;

            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");
            }

            if (serverOption.Prefixes == null || serverOption.Prefixes.Length == 0)
            {
                throw new ArgumentException("URI prefixes are required");
            }

            foreach (var prefix in serverOption.Prefixes)
            {
                logger.LogInformation($"Listen: {prefix}");
                listener.Prefixes.Add(prefix);
            }
        }


        public void Start()
        {
            listener.Start();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Proxy Server running at: {time}", DateTimeOffset.Now);
            }
        }

        public void Stop()
        {
            listener.Stop();
            listener.Close();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Start();

            try
            {
                while (!stoppingToken.IsCancellationRequested && listener.IsListening)
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
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }
                    finally
                    {
                        if (context != null)
                        {
                            context.Response.OutputStream.Close();
                        }
                    }

                    await Task.Delay(100, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            Stop();
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
            var request = Json.Deserialize<SqlRemoteRequest>(json);
            Console.WriteLine($"{DateTime.Now} [Req] {request}");

            SqlRemoteResult result = Execute(request);

            json = Json.Serialize(result);
            Console.WriteLine($"{DateTime.Now} [Ret] {result}");

            return json;
        }


        private SqlRemoteResult Execute(SqlRemoteRequest request)
        {
            DbAgent agent = CreateDbAgent();

            SqlRemoteHandler handler = new SqlRemoteHandler(agent);
            return handler.Execute(request);
        }

        private DbAgent CreateDbAgent()
        {
            string connectionString = serverOption.ConnectionString;
            DbAgent agent;
            switch (serverOption.Style)
            {
                case DbAgentStyle.SQLite:
                    agent = new SQLiteAgent(new SQLiteConnectionStringBuilder(connectionString));
                    break;

                default:
                    agent = new SqlDbAgent(new SqlConnectionStringBuilder(connectionString));
                    break;
            }

            return agent;
        }
    }
}
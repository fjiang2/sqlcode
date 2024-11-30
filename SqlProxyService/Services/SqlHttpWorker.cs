using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;

using SqlProxy.Service.Settings;

namespace SqlProxy.Service.Services
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
                throw new NotSupportedException("Needs Windows 10, Server 2008 or later.");
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
                        Run(context);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }
                    finally
                    {
                        context?.Response.OutputStream.Close();
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

        private void Run(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            string requestString = StreamToString(request.InputStream);

            byte[] buffer = Execute(requestString);

            HttpListenerResponse response = context.Response;
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private static string StreamToString(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private byte[] Execute(string requestString)
        {
            SqlRemoteProxy proxy = new SqlRemoteProxy(serverOption.DbServers);
            string responseString = proxy.Execute(requestString);

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            return buffer;
        }
    }
}
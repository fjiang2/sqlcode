using System;
using System.Data.SqlClient;

using Sys.Data.SqlRemote;
using Sys.Data.SqlClient;
using StackExchange.Redis;

namespace Sys.Data.SqlRedis
{
    public class SqlRedisServer   
    {
        public string SqlCommandChannel { get; set; } = "sqlremote-sql-command";
        public string SqlResultChannel { get; set; } = "sqlremote-sql-result";


        public ConnectionMultiplexer Redis { get; }

        private SqlRemoteHandler handler;

        public SqlRedisServer(ConfigurationOptions options, SqlConnectionStringBuilder connection)
        {
            handler = new SqlRemoteHandler(new SqlDbAgent(connection));

            this.Redis = ConnectionMultiplexer.Connect(options);
            ISubscriber sub = Redis.GetSubscriber();
            sub.Subscribe(SqlCommandChannel, (channel, value) =>
            {
                var result = Execute(value);
                sub.Publish(SqlResultChannel, result);
            });
        }

        public string Execute(string json)
        {
            Console.WriteLine($"{DateTime.Now} [Req] {json}");
            
            SqlRemoteRequest request = Json.Deserialize<SqlRemoteRequest>(json);
            SqlRemoteResult result = handler.Execute(request);
            
            Console.WriteLine($"{DateTime.Now} [Ret] {result}");
            json = Json.Serialize(result);
            return json;
        }
    }
}

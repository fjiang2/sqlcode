using System;
using System.Threading.Tasks;
using Sys.Data.SqlRemote;
using StackExchange.Redis;

namespace Sys.Data.SqlRemote
{

    public class SqlRedisBroker : ISqlRemoteBroker
    {
        public string SqlCommandChannel { get; set; } = "sqlremote-sql-command";
        public string SqlResultChannel { get; set; } = "sqlremote-sql-result";

        public ConnectionMultiplexer Redis { get; }
        private readonly ChannelMessageQueue queue;

        public SqlRedisBroker(ConfigurationOptions options)
        {
            this.Redis = ConnectionMultiplexer.Connect(options);

            ISubscriber sub = Redis.GetSubscriber();
            this.queue = sub.Subscribe(SqlResultChannel);
        }

        public DbAgentStyle Style { get; set; }

        public async Task<SqlRemoteResult> RequesteAsync(SqlRemoteRequest request)
        {
            string json = Json.Serialize(request);
            ISubscriber sub = Redis.GetSubscriber();
            sub.Publish(SqlCommandChannel, json);

            var message = await queue.ReadAsync();
            json = (string)message.Message;
            SqlRemoteResult result = Json.Deserialize<SqlRemoteResult>(json);
            return result;
        }
    }
}

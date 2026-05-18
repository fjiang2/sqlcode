namespace Sys.Data.SqlRemote
{
    public class DbProvider
    {
        /// <summary>
        /// Database server alias
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Style of database engine 
        /// </summary>
        public DbAgentStyle Style { get; set; }

        public DbProvider() 
        { 
        }

        public override string ToString() => ServerName;
    }
}

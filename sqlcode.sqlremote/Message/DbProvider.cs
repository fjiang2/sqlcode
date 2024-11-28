namespace Sys.Data.SqlRemote
{
    public class DbProvider
    {
        /// <summary>
        /// Database server alias
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Style of database engine 
        /// </summary>
        public DbAgentStyle Style { get; set; }
    }
}

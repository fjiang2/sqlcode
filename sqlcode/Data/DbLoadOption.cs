namespace Sys.Data
{
    public struct DbLoadOption
    {
        public static readonly DbLoadOption DefaultOption = new DbLoadOption
        {
            StartRecord = 0,
            MaxRecords = -1,
            Mode = DbLoadMode.DbFill
        };

        public DbLoadMode Mode { get; set; }

        /// <summary>
        /// start-record
        /// </summary>
        public int StartRecord { get; set; }

        /// <summary>
        /// Number of records. -1: all records from start-record
        /// </summary>
        public int MaxRecords { get; set; }
    }
}

namespace Sys.Data
{
    /// <summary>
    /// Define how to load rows from database server
    /// </summary>
    public struct DbLoadOption
    {
        public static readonly DbLoadOption DefaultOption = new DbLoadOption
        {
            StartRecord = 0,
            MaxRecords = -1,
            Mode = DbLoadMode.DbFill
        };

        /// <summary>
        /// Use database adapter or database reader
        /// </summary>
        public DbLoadMode Mode { get; set; }

        /// <summary>
        /// Start record
        /// The zero-based record number to start with
        /// </summary>
        public int StartRecord { get; set; }

        /// <summary>
        /// Maximum number of records to retrieve.
        /// Number of records. -1: all records from start-record
        /// </summary>
        public int MaxRecords { get; set; }
    }
}

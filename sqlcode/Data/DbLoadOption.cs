namespace Sys.Data
{
    public struct DbLoadOption
    {
        public static readonly DbLoadOption DefaultOption = new DbLoadOption
        {
            StartRecord = 0,
            MaxRecords = 0,
            Mode = DbLoadMode.DbFill
        };

        public DbLoadMode Mode { get; set; }
        public int StartRecord { get; set; }
        public int MaxRecords { get; set; }
    }
}

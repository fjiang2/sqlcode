using System.Data;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteParameter
    {
        public string ParameterName { get; set; }
        public object Value { get; set; }
        public ParameterDirection Direction { get; set; }

        public override string ToString()
        {
            if (Direction == ParameterDirection.Input)
                return $"{ParameterName} = in {Value}";
            else if (Direction == ParameterDirection.Output)
                return $"{ParameterName} = out {Value}";
            else if (Direction == ParameterDirection.InputOutput)
                return $"{ParameterName} = in-out {Value}";
            else
                return $"{ParameterName} = return {Value}";
        }
    }
}

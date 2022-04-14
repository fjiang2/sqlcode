using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.SqlRemote
{
    static class Extension
    {
        public static string GetAllMessages(this Exception exception)
        {
            const string ERROR_ONE_AND_MORE = "One or more errors occurred.";

            StringBuilder builder = new StringBuilder();

            if (exception.Message != ERROR_ONE_AND_MORE)
                builder.AppendLine(exception.Message);

            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                if (innerException.Message != ERROR_ONE_AND_MORE)
                    builder.AppendLine(innerException.Message);

                innerException = innerException.InnerException;
            }

            return builder.ToString();
        }
    }
}

global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Sys.Data;

#if NET8_0
global using Microsoft.Data.SqlClient;
#else
global using System.Data.SqlClient;
#endif

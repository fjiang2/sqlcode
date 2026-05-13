global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using SqlGrpcService.Settings;
global using Sys.Data;

#if NET10_0
#else
global using System.Data.SqlClient;
#endif

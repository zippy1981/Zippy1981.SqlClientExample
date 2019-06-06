using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Zippy1981.SqlClientExample.Startup))]

namespace Zippy1981.SqlClientExample
{
    public class Startup : FunctionsStartup
    {
        private const string ConnectionStringKey = "SqlConnectionString";
        private const string SqlTokenTenantKey = "SqlTenant";

        private static string ConnectionString =>
            Environment.GetEnvironmentVariable(ConnectionStringKey, EnvironmentVariableTarget.Process);

        /// <summary>
        /// The Tenant for the SQL Token
        /// </summary>
        /// <remarks>If the Service principle is a guest account, we must specify the tenant for this guest account.</remarks>
        private static string SqlTokenTenant =>
            Environment.GetEnvironmentVariable(SqlTokenTenantKey, EnvironmentVariableTarget.Process);

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddSingleton<SqlConnectionContext>(new SqlConnectionContext(ConnectionString, SqlTokenTenant));
        }
    }
}

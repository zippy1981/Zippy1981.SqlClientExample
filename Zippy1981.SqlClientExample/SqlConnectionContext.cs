using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Zippy1981.SqlClientExample.Startup))]

namespace Zippy1981.SqlClientExample
{
    using System.Data.SqlClient;
    using NextGenSqlConnection = Microsoft.Data.SqlClient.SqlConnection;
    using Microsoft.Azure.Services.AppAuthentication;
    using System.Threading.Tasks;

    public class SqlConnectionContext
    {
        private const string AzureSqlDbResource = "https://database.windows.net/";

        private string ConnectionString { get; }
        
        /// <summary>
        /// The name of the tenant we are autenticating against.
        /// </summary>
        /// <remarks>This is necessary for Guest users.</remarks>
        private string TenantName { get; }

        private async Task<string> GetAccessTokenAsync()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return await azureServiceTokenProvider.GetAccessTokenAsync(AzureSqlDbResource, TenantName);
        }

        public SqlConnectionContext(string connectionString, string tenant = null)
        {
            ConnectionString = connectionString;
            TenantName = tenant;
        }

        public async Task<SqlConnection> CreateConnection()
        {
            var cn = new SqlConnection(ConnectionString);
            cn.AccessToken = await GetAccessTokenAsync();
            return cn;
        }

        public async Task<NextGenSqlConnection> CreateNextGenConnection()
        {
            var cn = new NextGenSqlConnection(ConnectionString);
            cn.AccessToken = await GetAccessTokenAsync();
            return cn;
        }
    }
}

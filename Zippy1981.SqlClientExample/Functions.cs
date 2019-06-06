using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Data;

namespace Zippy1981.SqlClientExample
{

    public class Functions
    {
        public Functions(SqlConnectionContext sqlConnectionContext, ILogger<Functions> logger)
        {
            SqlContext = sqlConnectionContext;
            Logger = logger;
        }

        private async Task<IDbConnection> GetConnection(bool nextGen = false) => nextGen 
            ? (IDbConnection) await SqlContext.CreateNextGenConnection() 
            : await SqlContext.CreateConnection();

        private SqlConnectionContext SqlContext { get; }
        private ILogger<Functions> Logger { get; }

        [FunctionName("GetTables")]
        public async Task<IActionResult> GetTables(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            var sb = new StringBuilder();
            Logger.LogTrace("Creating Database connection object.");
            using (var cn = await GetConnection(req.Query.ContainsKey("nextGen")))
            {
                using (var cmd = cn.CreateCommand())
                {
                    cn.Open();
                    cmd.CommandText = "SELECT * FROM sys.tables";
                    using (var sqlRdr = cmd.ExecuteReader())
                        while (sqlRdr.Read())
                        {
                            for (var i = 0; i < sqlRdr.FieldCount; i++)
                            {
                                sb.AppendFormat("{0},", sqlRdr[i]);
                            }
                            sb.AppendLine();
                        }
                    cn.Close();
                }
            }

            var result = new ContentResult();
            result.Content = sb.ToString();
            return result;
        }
    }
}

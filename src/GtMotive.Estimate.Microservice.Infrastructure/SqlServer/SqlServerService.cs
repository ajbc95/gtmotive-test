using System.Data;
using GtMotive.Estimate.Microservice.Infrastructure.SqlServer.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace GtMotive.Estimate.Microservice.Infrastructure.SqlServer
{
    /// <summary>
    /// SQL Server service.
    /// </summary>
    public class SqlServerService(IOptions<SqlServerDbSettings> dbSettings)
    {
        public IDbConnection SqlServerConnection { get; private set; } = new SqlConnection(dbSettings?.Value.ConnectionString);
    }
}

using System;
using System.Data.SqlClient;

namespace ITechcg.Dal.ConnectionProviders
{
    /// <summary>
    /// Implement this interface to provide a way to get sql connection.
    /// IDataExecutionContext is dependent on this interface.
    /// </summary>
    public interface IConnectionProvider
    {
        SqlConnection GetConnection();
    }
}

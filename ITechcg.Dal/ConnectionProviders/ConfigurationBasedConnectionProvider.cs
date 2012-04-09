using System;
using System.Data;
using System.Data.SqlClient;

namespace ITechcg.Dal.ConnectionProviders
{
    using Exceptions;

    /// <summary>
    /// Creates a connection object based on the name of the connection string
    /// in the config file.
    /// </summary>
    public sealed class ConfigurationBasedConnectionProvider : IConnectionProvider
    {
        string connectionStringName;

        public ConfigurationBasedConnectionProvider(string ConnectionStringName)
        {
            connectionStringName = ConnectionStringName;
        }

        /// <summary>
        /// Method returns a connection object.
        /// </summary>
        public SqlConnection GetConnection()
        {
            try
            {
                SqlConnection con = new SqlConnection(connectionStringName);
                return con;
            }
            catch (Exception ex)
            {
                throw new DalException(ex, "Could not create connection from the provided connection string name {0}", connectionStringName);
            }
        }
    }
}

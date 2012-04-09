using System;
using System.Collections.Generic;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ITechcg.Dal
{
    using ConnectionProviders;
    using Exceptions;

    /// <summary>
    /// Data operations executor that doesn't support transactions.
    /// Use TransactionExecutor class for transaction support.
    /// </summary>
    public sealed class NonTransactionalExecutionContext : IDataExecutionContext
    {
        private SqlConnection _connection;

        private IConnectionProvider connectionProvider;

        public IConnectionProvider ConnectionProvider
        {
            get { return connectionProvider; }
            set { connectionProvider = value; }
        }

        /// <summary>
        /// Instantiates the class with the provided connection provider.
        /// </summary>
        public NonTransactionalExecutionContext(IConnectionProvider ConnectionProvider)
        {
            connectionProvider = ConnectionProvider;
        }

        /// <summary>
        /// Initializes a SqlConnection either for transaction or plain
        /// </summary>
        private void InitializeConnection()
        {
            if (this._connection != null && this._connection.State != ConnectionState.Closed)
            {
                throw new DalException("Previous connection was not closed. Please close previous connetion. Check SDK and make sure all execute sql commands are closing connection for non transaction methods.");
            }

            _connection = ConnectionProvider.GetConnection();

            //Open the connection if not in transaction
            _connection.Open();
        }

        /// <summary>
        /// Closes a connetion if connection is not associated with a transaction.
        /// For transaction specific Connection, only commit or rollback will close the connection.
        /// </summary>        
        private void DisposeConnection()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
                _connection.Close();
        }

        #region IDataExecutionContext Members

        /// <summary>
        /// Creates a raw TaskSqlCommand
        /// </summary>
        public TaskSqlCommand CreateCommand()
        {
            this.InitializeConnection();
            SqlCommand cmd = this._connection.CreateCommand();
            TaskSqlCommand tcmd = new TaskSqlCommand(cmd);
            return tcmd;
        }

        /// <summary>
        /// Creates a TaskSqlCommand for a stored procedure provided
        /// </summary>
        public TaskSqlCommand CreateStoredProcCommand(string storedProcName)
        {
            this.InitializeConnection();
            SqlCommand cmd = this._connection.CreateCommand();
            cmd.CommandText = storedProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            TaskSqlCommand tcmd = new TaskSqlCommand(cmd);
            return tcmd;
        }

        /// <summary>
        /// Creates a TaskSqlCommand for a string sql command.
        /// </summary>
        /// <remarks>Avoid using this method.</remarks>
        public TaskSqlCommand CreateTextCommand(string commandText)
        {
            this.InitializeConnection();
            SqlCommand cmd = this._connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.Text;
            TaskSqlCommand tcmd = new TaskSqlCommand(cmd);
            return tcmd;
        }

        /// <summary>
        /// Creates a TaskSqlCommand for a string sql command.
        /// </summary>
        /// <remarks>Avoid using this method.</remarks>
        public TaskSqlCommand CreateTextCommand(string commandFormatText, params object[] args)
        {
            this.InitializeConnection();
            SqlCommand cmd = this._connection.CreateCommand();            
            cmd.CommandText = string.Format(commandFormatText, args);
            cmd.CommandType = CommandType.Text;
            TaskSqlCommand tcmd = new TaskSqlCommand(cmd);
            return tcmd;
        }

        /// <summary>
        /// Executes command as Non Query. Best for Update/Delete operations.
        /// </summary>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(TaskSqlCommand cmd)
        {
            try
            {
                int ret = cmd.Command.ExecuteNonQuery();                
                return ret;
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 50000)
                {
                    throw new UserFriendlyDBException(sqlex.Message, sqlex);
                }
                else
                {
                    throw new DalException(sqlex, "SqlException occured in ExectuteNonQuery. {0}", sqlex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new DalException(ex, "Error occured in ExecuteNonQuery. {0}", ex.Message);
            }
            finally
            {
                this.DisposeConnection();
            }
        }

        /// <summary>
        /// Executes command as that returns a single value
        /// </summary>
        public object ExecuteScalar(TaskSqlCommand cmd)
        {
            try
            {
                object ret = cmd.Command.ExecuteScalar();
                return ret;
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 50000)
                {
                    throw new UserFriendlyDBException(sqlex.Message, sqlex);
                }
                else
                {
                    throw new DalException(sqlex, "SqlException occured in ExecuteScalar. {0}", sqlex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new DalException(ex, "Error occured in ExecuteScalar. {0}", ex.Message);
            }
            finally
            {
                this.DisposeConnection();
            }
        }

        /// <summary>
        /// Method is implemented to return an XML reader
        /// </summary>
        /// <remarks>Caller should explicitly close the connection for non transactional excution context.</remarks>
        public XmlReader ExecuteXmlReader(TaskSqlCommand cmd)
        {
            try
            {
                XmlReader reader = cmd.Command.ExecuteXmlReader();
                return reader;
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 50000)
                {
                    throw new UserFriendlyDBException(sqlex.Message, sqlex);
                }
                else
                {
                    throw new DalException(sqlex, "SqlException occured in ExecuteXmlReader. {0}", sqlex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new DalException(ex, "Error occured in ExecuteXmlReader. {0}", ex.Message);
            }
        }

        /// <summary>
        /// Executes command as DataAdapter.
        /// </summary>
        /// <remarks>Avoid this if you need DataTable or DataSet. Other methods are exposed for returning DataTable and DataSet</remarks>
        public SqlDataAdapter ExecuteDataAdapter(TaskSqlCommand cmd)
        {
            try
            {
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd.Command);
                return da;
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 50000)
                {
                    throw new UserFriendlyDBException(sqlex.Message, sqlex);
                }
                else
                {
                    throw new DalException(sqlex, "SqlException occured in ExecuteDataAdapter. {0}", sqlex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new DalException(ex, "Error occured in ExecuteDataAdapter. {0}", ex.Message);
            }
            finally
            {
                this.DisposeConnection();
            }
        }

        /// <summary>
        /// Executes command as DataTable. For strongly typed programming model this is not recommended.
        /// </summary>
        public DataTable ExecuteDataTable(TaskSqlCommand cmd)
        {           
            try
            {
                DataTable dt = new DataTable();
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd.Command);
                da.Fill(dt);
                return dt;
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 50000)
                {
                    throw new UserFriendlyDBException(sqlex.Message, sqlex);
                }
                else
                {
                    throw new DalException(sqlex, "SqlException occured in ExecuteDataTable. {0}", sqlex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new DalException(ex, "Error occured in ExecuteDataTable. {0}", ex.Message);
            }
            finally
            {
                this.DisposeConnection();
            }          
        }

        /// <summary>
        /// Executes command as DataSet. For strongly typed programming model this is not recommended.
        /// </summary>
        public DataSet ExecuteDataSet(TaskSqlCommand cmd)
        {
            try
            {
                DataSet ds = new DataSet();
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd.Command);
                da.Fill(ds);
                return ds;
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 50000)
                {
                    throw new UserFriendlyDBException(sqlex.Message, sqlex);
                }
                else
                {
                    throw new DalException(sqlex, "SqlException occured in ExecuteDataSet. {0}", sqlex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new DalException(ex, "Error occured in ExecuteDataSet. {0}", ex.Message);
            }
            finally
            {
                this.DisposeConnection();
            }
        }

        /// <summary>
        /// Provides a forward-readonly access to the database. This is the fasted way to retrieve data. ExecuteEntityList uses this
        /// method to map EntityBase from the raw data.
        /// </summary>        
        /// <remarks>Caller should not close the connection. This method takes care of that.</remarks>
        public SqlDataReader ExecuteDataReader(TaskSqlCommand cmd)
        {
            try
            {
                System.Data.SqlClient.SqlDataReader dr = cmd.Command.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 50000)
                {
                    throw new UserFriendlyDBException(sqlex.Message, sqlex);
                }
                else
                {
                    throw new DalException(sqlex, "SqlException occured in ExecuteDataReader. {0}", sqlex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new DalException(ex, "Error occured in ExecuteDataReader. {0}", ex.Message);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

            this.DisposeConnection();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

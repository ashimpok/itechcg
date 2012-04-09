using System;
using System.Xml;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ITechcg.Dal
{
    using ConnectionProviders;
    using Exceptions;

    /// <summary>
    /// Data operations executor that support transactions.
    /// Use NonTransactionalExectutionContext if transaction is not required.
    /// </summary>
    /// <example>
    /// An example of how transaction Executor can be used. Usually initiated from Service/Controller as they can batch things together.
    /// <code>
    ///     //Initiate a transaction
    ///     TransactionExecutionContext dbContext = new TransactionExecutionContext();
    ///     CategoryModel transAwareCategoryModel = new CategoryModel(dbContext);
    ///     try
    ///     {
    ///         dbContext.BeginTransaction();
    ///         while(some thing)
    ///         {
    ///             if(! delete condition){
    ///                 transAwareCategoryModel.SaveParticipantDetails(participantId, subCategoryId, txtStringData.Text, date, chBoolData.Checked);
    ///             }
    ///             else
    ///                 transAwareCategoryModel.DeleteParticipantDetails(participantId, subCategoryId);
    ///         }
    ///         dbContext.CommitTransaction();
    ///         
    ///     }catch (Exception){
    ///         dbContext.RollbackTransaction();
    ///     }
    /// </code>
    /// </example>
    public sealed class TransactionExecutionContext : IDataExecutionContext
    {
        private SqlTransaction _transaction;
        private SqlConnection _connectionForTransaction;

        private IConnectionProvider connectionProvider;

        /// <summary>
        /// Gets/Sets connection provider. If this is not set the constructor will initialize DefaultConnectionProvider
        /// </summary>
        public IConnectionProvider ConnectionProvider
        {
            get { return connectionProvider; }
            set { connectionProvider = value; }
        }

        private bool _transactionInProgress;

        public bool TransactionInProgress
        {
            get { return _transactionInProgress; }
        }

        /// <summary>
        /// Instantiates the class with the the default connection provider.
        /// </summary>
        public TransactionExecutionContext(IConnectionProvider ConnectionProvider)
        {
            connectionProvider = ConnectionProvider;
        }

        /// <summary>
        /// Starts a transaction
        /// </summary>
        public void BeginTransaction()
        {
            if (_transactionInProgress)
                throw new DalException("Can not start a transaction at this point. Another transaction is in process");

            _connectionForTransaction = ConnectionProvider.GetConnection();
            _connectionForTransaction.Open();
            _transaction = _connectionForTransaction.BeginTransaction();
            _transactionInProgress = true;
        }

        /// <summary>
        /// Commits changes to the database.
        /// </summary>
        /// <exception cref="Exceptions.InvalidTransactionOperation">Thrown if the transaction has not started yet. Use BeginTransaction before call this method.</exception>
        public void CommitTransaction()
        {
            if (!_transactionInProgress)
                throw new DalException("Not in a transaction");

            _transaction.Commit();
            _connectionForTransaction.Close();

            _transaction.Dispose();
            _connectionForTransaction.Dispose();
            _transactionInProgress = false;
        }

        /// <summary>
        /// Rollbacks changes to the database since the last time a commit was performed.
        /// </summary>
        /// <exception cref="Exceptions.InvalidTransactionOperation">Thrown if transaction has not been started.</exception>
        public void RollbackTransaction()
        {
            if (!_transactionInProgress)
                throw new DalException("Not in a transaction");

            try
            {
                _transaction.Rollback();
                _connectionForTransaction.Close();

                _transaction.Dispose();
                _connectionForTransaction.Dispose();
            }
            catch(Exception ex)
            {
                throw new DalException(ex, "Error occured when rolling back transaction. {0}", ex.Message);
            }
            finally
            {
                _transactionInProgress = false;
            }
        }

        #region IDataExecutionContext Members

        /// <summary>
        /// Creates a raw TaskSqlCommand
        /// </summary>
        public TaskSqlCommand CreateCommand()
        {
            SqlCommand cmd = this._connectionForTransaction.CreateCommand();            
            TaskSqlCommand tcmd = new TaskSqlCommand(cmd, _transaction);
            return tcmd;
        }

        /// <summary>
        /// Creates a TaskSqlCommand for a stored procedure provided
        /// </summary>
        public TaskSqlCommand CreateStoredProcCommand(string storedProcName)
        {
            SqlCommand cmd = this._connectionForTransaction.CreateCommand();
            cmd.CommandText = storedProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            
            TaskSqlCommand tcmd = new TaskSqlCommand(cmd, _transaction);
            return tcmd;
        }

        /// <summary>
        /// Creates a TaskSqlCommand for a string sql command.
        /// </summary>
        /// <remarks>Avoid using this method.</remarks>
        public TaskSqlCommand CreateTextCommand(string commandText)
        {
            SqlCommand cmd = this._connectionForTransaction.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.Text;
            
            TaskSqlCommand tcmd = new TaskSqlCommand(cmd, _transaction);
            return tcmd;
        }

        /// <summary>
        /// Creates a TaskSqlCommand for a string sql command.
        /// </summary>
        /// <remarks>Avoid using this method.</remarks>
        public TaskSqlCommand CreateTextCommand(string commandFormatText, params object[] args)
        {
            SqlCommand cmd = this._connectionForTransaction.CreateCommand();
            cmd.CommandText = string.Format(commandFormatText, args);
            cmd.CommandType = CommandType.Text;
            
            TaskSqlCommand tcmd = new TaskSqlCommand(cmd, _transaction);
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
                return cmd.Command.ExecuteNonQuery();
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
        }
        
        ///<summary>
        /// Executes command and returns a single object
        /// </summary>
        public object ExecuteScalar(TaskSqlCommand cmd)
        {
            try
            {
                return cmd.Command.ExecuteScalar();
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
        }

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
        }

        /// <summary>
        /// Provides a forward-readonly access to the database. This is the fasted way to retrieve data.
        /// </summary>        
        public SqlDataReader ExecuteDataReader(TaskSqlCommand cmd)
        {
            try
            {
                System.Data.SqlClient.SqlDataReader dr = cmd.Command.ExecuteReader();
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
            if(_transactionInProgress)
                this.RollbackTransaction();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

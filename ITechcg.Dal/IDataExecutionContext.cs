using System;
using System.Collections.Generic;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

namespace ITechcg.Dal
{
    using ConnectionProviders;

    /// <summary>
    /// Interface defines functionality required to perform a DAO operation.
    /// There are two implementations of this executor. One for transaction aware tasks another for transaction unaware tasks.
    /// </summary>
    /// <seealso cref="SimpleExecutor"/>
    /// <seealso cref="TransactionAwareExecutor"/>
    public interface IDataExecutionContext : IDisposable
    {
        int ExecuteNonQuery(TaskSqlCommand cmd);
        object ExecuteScalar(TaskSqlCommand cmd);
        SqlDataAdapter ExecuteDataAdapter(TaskSqlCommand cmd);
        DataTable ExecuteDataTable(TaskSqlCommand cmd);
        DataSet ExecuteDataSet(TaskSqlCommand cmd);
        SqlDataReader ExecuteDataReader(TaskSqlCommand cmd);

        /// <summary>
        /// Method is implemented to return an XML reader
        /// </summary>
        /// <remarks>Caller should explicitly close the connection for non transactional excution context.</remarks>
        XmlReader ExecuteXmlReader(TaskSqlCommand cmd);

        TaskSqlCommand CreateCommand();
        TaskSqlCommand CreateStoredProcCommand(string storedProcName);
        TaskSqlCommand CreateTextCommand(string commandText);
        TaskSqlCommand CreateTextCommand(string commandFormatString, params object[] args);

        IConnectionProvider ConnectionProvider { get;set;}
    }
}

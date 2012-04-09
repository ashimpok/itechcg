using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITechcg.Dal
{
    /// <summary>
    /// This is an interface used by Dao objects
    /// </summary>
    /// <example>
    /// Following example shows how to use ITechcg.Dal objects
    /// <code>
    /// public class GisDao : IDao (or IGisDao that derives from IDao)
    /// {
    ///    public GisDao(IDataExecutionContext dataExecutionContext)
    ///    {
    ///        this.dataExecutionContext = dataExecutionContext;
    ///    }
    ///
    ///    public DataSet GetCountrySpecificGis(string cntryCode)
    ///    {
    ///        IDataExecutionContext dataContext = this.DataExecutionContext;
    ///        TaskSqlCommand cmd = dataContext.CreateStoredProcCommand("GetCountrySpecificGis");
    ///        cmd.CreateParameter("CNTRY_CODE", cntryCode);
    ///
    ///        return dataContext.ExectuteDataSet(cmd);
    ///    }
    ///
    ///    #region IDataAccessObject Members
    ///
    ///    IDataExecutionContext dataExecutionContext;
    ///    public IDataExecutionContext DataExecutionContext
    ///    {
    ///        get
    ///        {
    ///            return dataExecutionContext;
    ///        }
    ///    }
    ///
    ///    #endregion
    /// }
    /// </code>
    /// </example>
    public interface IDao
    {
        IDataExecutionContext DataExecutionContext { get; }
    }
}

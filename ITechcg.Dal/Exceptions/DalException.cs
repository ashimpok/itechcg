using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITechcg.Infrastructure.Exceptions;

namespace ITechcg.Dal.Exceptions
{
    /// <summary>
    /// Base exception from this application layer
    /// </summary>
    public class DalException : ITechcgException
    {
        public DalException(string message) 
            : base(message) 
        { 
        }

        public DalException(string FormatString, params object[] Args)
            : base(FormatString, Args)
        {
        }

        public DalException(Exception InnerException, string Message) 
            : base(InnerException, Message)
        {
        }

        public DalException(Exception InnerException, string FormatString, params object[] Args)
            : base(InnerException, FormatString, Args)
        {
        }
    }
}

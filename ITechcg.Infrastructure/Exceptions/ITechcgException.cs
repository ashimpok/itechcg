using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITechcg.Infrastructure.Exceptions
{
    /// <summary>
    /// Base exception for all ITechcg thrown exceptions
    /// </summary>
    public class ITechcgException : ApplicationException
    {
        /// <summary>
        /// Instantiates exception with the provided message
        /// </summary>        
        public ITechcgException(string Message) 
            : base(Message) 
        {
        }

        /// <summary>
        /// Instantiates exception with provided format string and arguments. Internally calls
        /// string.Format. This is provided for ease of use.
        /// </summary>
        public ITechcgException(string FormatString, params object[] Args)
            : base(string.Format(FormatString, Args))
        {
        }

        /// <summary>
        /// Instantiates exception with innerException as inner exception with the provided message.
        /// </summary>
        public ITechcgException(Exception InnerException, string Message) 
            : base(Message, InnerException)
        {
        }

        /// <summary>
        /// Instantiates exception with innerException as inner exception with the provided formatted string.
        /// Internally uses string.Format to create the error message.
        /// </summary>
        public ITechcgException(Exception InnerException, string FormatString, params object[] Args)
            : base (string.Format(FormatString, Args), InnerException)
        {
        }
    }
}

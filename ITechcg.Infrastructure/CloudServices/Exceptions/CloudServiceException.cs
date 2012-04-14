using System;
using System.Collections.Generic;
using System.Text;

using ITechcg.Infrastructure.Exceptions;

namespace ITechcg.Infrastructure.CloudServices.Exceptions
{
    /// <summary>
    /// Base class for cloud services related exception.
    /// </summary>
    public class CloudServiceException : ITechcgException
    {
        /// <summary>
        /// Instantiates exception with the provided message
        /// </summary>        
        public CloudServiceException(string Message)
            : base(Message) 
        {
        }

        /// <summary>
        /// Instantiates exception with provided format string and arguments. Internally calls
        /// string.Format. This is provided for ease of use.
        /// </summary>
        public CloudServiceException(string FormatString, params object[] Args)
            : base(FormatString, Args)
        {
        }

        /// <summary>
        /// Instantiates exception with innerException as inner exception with the provided message.
        /// </summary>
        public CloudServiceException(Exception InnerException, string Message)
            : base(InnerException, Message)
        {
        }

        /// <summary>
        /// Instantiates exception with innerException as inner exception with the provided formatted string.
        /// Internally uses string.Format to create the error message.
        /// </summary>
        public CloudServiceException(Exception InnerException, string FormatString, params object[] Args)
            : base(InnerException, FormatString, Args)
        {
        }
    }
}

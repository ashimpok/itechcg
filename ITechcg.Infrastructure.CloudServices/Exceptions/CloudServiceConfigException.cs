using System;
using System.Collections.Generic;
using System.Text;

namespace ITechcg.Infrastructure.CloudServices.Exceptions
{
    public class CloudServiceConfigException : CloudServiceException
    {
        /// <summary>
        /// Instantiates exception with the provided message
        /// </summary>        
        public CloudServiceConfigException(string Message)
            : base(Message)
        {
        }

        /// <summary>
        /// Instantiates exception with provided format string and arguments. Internally calls
        /// string.Format. This is provided for ease of use.
        /// </summary>
        public CloudServiceConfigException(string FormatString, params object[] Args)
            : base(string.Format(FormatString, Args))
        {
        }

        /// <summary>
        /// Instantiates exception with innerException as inner exception with the provided message.
        /// </summary>
        public CloudServiceConfigException(Exception InnerException, string Message) 
            : base(Message, InnerException)
        {
        }

        /// <summary>
        /// Instantiates exception with innerException as inner exception with the provided formatted string.
        /// Internally uses string.Format to create the error message.
        /// </summary>
        public CloudServiceConfigException(Exception InnerException, string FormatString, params object[] Args)
            : base (string.Format(FormatString, Args), InnerException)
        {
        }
    }
}

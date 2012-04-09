using System;

namespace ITechcg.Dal.Exceptions
{
    /// <summary>
    /// Any database related user encoded exception that do not need further processing.
    /// </summary>
    public class UserFriendlyDBException : DalException
    {
        public UserFriendlyDBException(string message) : base(message) { }

        public UserFriendlyDBException(string FormatString, params object[] Args)
            : base(FormatString, Args)
        {
        }
        
        public UserFriendlyDBException(Exception InnerException, string Message) 
            : base(Message, InnerException)
        {
        }

        public UserFriendlyDBException(Exception InnerException, string FormatString, params object[] Args)
            : base (InnerException, FormatString, Args)
        {
        }
    }
}

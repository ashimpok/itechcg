using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using ITechcg.Infrastructure.EmailServices.ServiceModel;

namespace ITechcg.Infrastructure.EmailServices.ContentProviders
{
    /// <summary>
    /// Interface provides information for an email.
    /// This is to be implemented by specific providers. See XMLEmailContentProvider for details.
    /// </summary>
    /// <remarks>Implmentors must have a parameter less constructor.</remarks>    
    public interface IEmailContentProvider
    {
        EmailContent Content { get; }

        /// <summary>
        /// Implement this method to accept data as string and initialize the email info provider. For example for XMLEmailInfoProvider this is an xml  file name.
        /// </summary>
        /// <param name="data"></param>
        void Initialize(string data);
    }
}

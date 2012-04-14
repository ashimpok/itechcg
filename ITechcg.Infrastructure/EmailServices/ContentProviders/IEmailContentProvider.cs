using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace ITechcg.Infrastructure.EmailServices.ContentProviders
{
    /// <summary>
    /// Interface provides information for an email.
    /// This is to be implemented by specific providers. See XMLEmailContentProvider for details.
    /// </summary>
    /// <remarks>Implmentors must have a parameter less constructor.</remarks>    
    public interface IEmailContentProvider
    {
        /// <summary>
        /// Gets the subject of the email. Can contain place holder keys.
        /// </summary>
        string Subject { get;}

        /// <summary>
        /// If provided will override the ones in the request.
        /// </summary>
        string SenderEmail { get; }

        /// <summary>
        /// Gets the plain text template body of the email, in case full HTML is not supported by the receipent.
        /// </summary>
        string PlainTextTemplateBody { get;}

        /// <summary>
        /// Gets the HTML template body of the email.
        /// </summary>
        string HTMLTemplateBody { get;}

        /// <summary>
        /// List of all linked resources that will be attached to the html view.
        /// </summary>
        /// <remarks>Never return a null. At least return an empty list.</remarks>
        List<LinkedResource> LinkedResources { get;}

        /// <summary>
        /// Implement this method to accept data as string and initialize the email info provider. For example for XMLEmailInfoProvider this is an xml  file name.
        /// </summary>
        /// <param name="data"></param>
        void Initialize(string data);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace ITechcg.Infrastructure.EmailServices.ServiceModel
{
    /// <summary>
    /// Class encapsulates email content. Content provider will provide instance of this class for users.
    /// </summary>
    public class EmailContent
    {
        public EmailContent()
        {
            linkedResources = new List<LinkedResource>();
        }

        /// <summary>
        /// Gets the subject of the email. Can contain place holder keys.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// If provided will override the ones in the request.
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// Gets the plain text template body of the email, in case full HTML is not supported by the receipent.
        /// </summary>
        public string PlainTextTemplateBody { get; set; }

        /// <summary>
        /// Gets the HTML template body of the email.
        /// </summary>
        public string HTMLTemplateBody { get; set; }

        IList<LinkedResource> linkedResources;

        /// <summary>
        /// List of all linked resources that will be attached to the html view.
        /// </summary>
        /// <remarks>Never return a null. At least return an empty list.</remarks>
        public IList<LinkedResource> LinkedResources 
        {
            get
            {
                ///TODO: Return readonly collection
                return linkedResources;
            }
        }

        public void AddLinkedResource(LinkedResource linkedResource)
        {
            this.linkedResources.Add(linkedResource);
        }
    }
}

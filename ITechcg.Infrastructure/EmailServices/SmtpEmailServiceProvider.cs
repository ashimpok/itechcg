using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace ITechcg.Infrastructure.EmailServices
{
    /// <summary>
    /// Email provider that uses SMTP to deliver the email
    /// </summary>
    public class SmtpEmailServiceProvider : IEmailServiceProvider
    {
        SmtpClient client;
        public SmtpEmailServiceProvider(SmtpClient Client) 
        {
            client = Client;
        }

        #region IEmailServiceProvider Members

        public void SendEmail(MailMessage Message)
        {            
            client.Send(Message);
        }

        #endregion
    }
}
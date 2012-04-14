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
        public SmtpEmailServiceProvider() { }

        #region IEmailServiceProvider Members

        public void SendEmail(MailMessage Message)
        {
            ///TODO: Relies on Smtp Setting on the config file
            ///Remove this dependency
            SmtpClient client = new SmtpClient();
            client.Send(Message);
        }

        #endregion
    }
}
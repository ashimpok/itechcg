using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace ITechcg.Infrastructure.EmailServices
{
    /// <summary>
    /// Interface is used by Email Processor to deal with the MailMessage after it is ready for sending.
    /// SmtpEmailServiceProvider class implements the SMTP send thus delivering the email right away.
    /// </summary>
    public interface IEmailServiceProvider
    {
        void SendEmail(MailMessage Mail);
    }
}

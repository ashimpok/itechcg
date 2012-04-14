using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITechcg.Infrastructure.CloudServices;
using System.Net.Mail;

namespace ITechcg.Infrastructure.EmailServices
{
    /// <summary>
    /// Uses cloud service to send email. Currently implements Amazon SES.
    /// This is great for bulk emailing.
    /// </summary>
    public class CloudEmailServiceProvider : IEmailServiceProvider
    {
        ICloudServiceConfigProvider cloudServiceConfigProvider;

        /// <summary>
        /// Instantiates this class using the provided config provider.
        /// </summary>
        public CloudEmailServiceProvider(ICloudServiceConfigProvider CloudServiceConfigProvider)
        {
            cloudServiceConfigProvider = CloudServiceConfigProvider;
        }

        /// <summary>
        /// Instantiates this class using the configuration file based config provider.
        /// </summary>
        public CloudEmailServiceProvider()
        {
            cloudServiceConfigProvider = new CloudServiceConfigProviderBase();
        }

        public void SendEmail(MailMessage Mail)
        {
            try
            {
                //Send email using cloud service
                CloudServiceClient cloudClient = CloudServiceFactory.CreateCloudServiceClient(this.cloudServiceConfigProvider);
                cloudClient.SendBulkEmail(Mail);
            }
            catch (CloudServices.Exceptions.CloudServiceException cse)
            {
                throw new EmailServices.Exceptions.EmailServiceException(cse, "Error when sending email");
            }
            catch (Exception ex)
            {
                throw new EmailServices.Exceptions.EmailServiceException(ex, "Error when sending email");
            }
        }
    }
}

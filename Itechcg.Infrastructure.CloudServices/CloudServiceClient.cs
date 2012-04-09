using System;
using System.Collections.Generic;
using System.Text;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Specialized;
using Amazon.S3.Transfer;
using System.IO;

namespace ITechcg.Infrastructure.CloudServices
{
    using Exceptions;
    using System.Net.Mail;
    using Amazon.SimpleEmail.Model;
    using Amazon.SimpleEmail;
    
    public class CloudServiceClient
    {
        ICloudServiceConfigProvider cloudServiceConfigProvider;

        private ICloudServiceConfigProvider CloudServiceConfigProvider
        {
            set { cloudServiceConfigProvider = value; }
        }

        /// <summary>
        /// Factory method to create Amazon Client
        /// </summary>
        /// <param name="cloudServiceConfigProvider">Cloud service configuration provider. Use the default Create method if you want to use web.config based provider and where all the keys specified in DefaultCloudServiceConfigProvider exactly matches the key name you have in the config file.</param>
        public static CloudServiceClient CreateCloudServiceClient(ICloudServiceConfigProvider cloudServiceConfigProvider)
        {
            CloudServiceClient client = new CloudServiceClient();
            client.CloudServiceConfigProvider = cloudServiceConfigProvider;

            return client;
        }

        /// <summary>
        /// Factory method to create Amazon Client with default configuration provider.
        /// Use this if you want to use web.config based provider and where all the keys specified in DefaultCloudServiceConfigProvider exactly matches the key name you have in the config file.
        /// </summary>        
        public static CloudServiceClient CreateCloudServiceClient()
        {
            CloudServiceClient client = new CloudServiceClient();
            client.CloudServiceConfigProvider = new CloudServiceConfigProviderBase();

            return client;
        }

        /// <summary>
        /// Can not create this class without the factory methods.
        /// </summary>
        private CloudServiceClient()
        {
        }

        #region Amazon S3 (Simple Storage Service) methods
        /// <summary>
        /// Enables versioning on the bucket. Once versioning is enabled you can not "un-version" the bucket. You can however suspend versioning on the bucket.
        /// </summary>
        public void EnableBucketVersioning()
        {
            try
            {
                using (AmazonS3 s3 = AWSClientFactory.CreateAmazonS3Client(this.cloudServiceConfigProvider.AWSAccessKeyId, this.cloudServiceConfigProvider.AWSSecretKey))
                {
                    S3BucketVersioningConfig versioningConfig = new S3BucketVersioningConfig()
                        .WithStatus("Enabled");
                    SetBucketVersioningRequest setBucketVersioningReq = new SetBucketVersioningRequest()
                        .WithBucketName(this.cloudServiceConfigProvider.AmazonBucket)
                        .WithVersioningConfig(versioningConfig);

                    s3.SetBucketVersioning(setBucketVersioningReq);
                }
            }
            catch (Exception ex)
            {
                throw new CloudServiceException(ex, "Error occured when enabling versioning on the bucket {0}", this.cloudServiceConfigProvider.AmazonBucket);
            }
        }

        /// <summary>
        /// Suspends versioning on a bucket. This method is not recommented.
        /// </summary>
        private void SuspendBucketVersioning()
        {
            try
            {
                using (AmazonS3 s3 = AWSClientFactory.CreateAmazonS3Client(this.cloudServiceConfigProvider.AWSAccessKeyId, this.cloudServiceConfigProvider.AWSSecretKey))
                {
                    S3BucketVersioningConfig versioningConfig = new S3BucketVersioningConfig()
                        .WithStatus("Suspended");

                    SetBucketVersioningRequest setBucketVersioningReq = new SetBucketVersioningRequest()
                        .WithBucketName(this.cloudServiceConfigProvider.AmazonBucket)
                        .WithVersioningConfig(versioningConfig);

                    s3.SetBucketVersioning(setBucketVersioningReq);
                }
            }
            catch (Exception ex)
            {
                throw new CloudServiceException(ex, "Error occured when enabling versioning on the bucket {0}", this.cloudServiceConfigProvider.AmazonBucket);
            }
        }

        /// <summary>
        /// Method retrives URL to an object in Amazon S3 for the Bucket name provided by IAmazonConfigProvider and identified by
        /// the keyName parameter. The URL returned will expire in the time provided. Returns null if url is not found.
        /// </summary>
        public string GetPreSignedUrl(string keyName, DateTime expirationTime)
        {
            try
            {
                using (AmazonS3 s3 = AWSClientFactory.CreateAmazonS3Client(this.cloudServiceConfigProvider.AWSAccessKeyId, this.cloudServiceConfigProvider.AWSSecretKey))
                {
                    GetPreSignedUrlRequest preSignedUrlReq = new GetPreSignedUrlRequest()
                        .WithBucketName(this.cloudServiceConfigProvider.AmazonBucket)
                        .WithKey(keyName)
                        .WithProtocol(Protocol.HTTPS)
                        .WithVerb(HttpVerb.GET)
                        .WithExpires(expirationTime);

                    string url = s3.GetPreSignedURL(preSignedUrlReq);

                    return url;
                }
            }
            catch (Exception ex)
            {
                throw new CloudServiceException(ex, "Unable to retrieve signed URL for {0}", keyName);
            }
        }

        /// <summary>
        /// Method deletes the provided keyName object from Cloud.
        /// </summary>
        public bool DeleteObject(string keyName)
        {
            try
            {
                using (AmazonS3 s3 = AWSClientFactory.CreateAmazonS3Client(this.cloudServiceConfigProvider.AWSAccessKeyId, this.cloudServiceConfigProvider.AWSSecretKey))
                {
                    DeleteObjectRequest deleteObjReq = new DeleteObjectRequest()
                        .WithBucketName(this.cloudServiceConfigProvider.AmazonBucket)
                        .WithKey(keyName);

                    DeleteObjectResponse deleteObjRes = s3.DeleteObject(deleteObjReq);

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new CloudServiceException(ex, "Unable to retrieve signed URL for {0}", keyName);
            }
        }

        /// <summary>
        /// Method retrives URL to an object in Amazon S3 for the Bucket name provided by IAmazonConfigProvider and identified by
        /// the keyName parameter. The URL returned will expire in the default time period provided by IAmazonConfigProvider or
        /// in 1 day if config is empty. Returns null if url is not found.
        /// </summary>
        public string GetPreSignedUrl(string keyName)
        {
            DateTime expirationTime = DateTime.Now;
            if (cloudServiceConfigProvider.DefaultExpirationDaysForUrl.HasValue && cloudServiceConfigProvider.DefaultExpirationDaysForUrl.Value > 0)
            {
                expirationTime = expirationTime.Date.AddDays(cloudServiceConfigProvider.DefaultExpirationDaysForUrl.Value);
            }
            else
            {
                expirationTime = expirationTime.AddDays(1);
            }

            return GetPreSignedUrl(keyName, expirationTime);
        }

        /// <summary>
        /// Uploads a file to the cloud service and assiciates it with the meta data provided. To use upload
        /// feature make sure configuration provider has AmazonBucket specified.
        /// </summary>
        /// <param name="fileFullPath">Full path to the file that is to be uploaded.</param>
        /// <param name="newFileName">Name of the file on cloud.</param>
        /// <param name="metaData">Any meta data you want to be associated with the file.</param>
        public void UploadFile(string fileFullPath, string newFileName, NameValueCollection metaData)
        {
            if (!File.Exists(fileFullPath))
                throw new CloudServiceException("Specified file does not exist.");

            try
            {
                using (FileStream fsStream = File.OpenRead(fileFullPath))
                {
                    UploadFile(fsStream, newFileName, metaData);
                }

            }
            catch (CloudServiceException aex)
            {
                throw aex;
            }
            catch (Exception ex)
            {
                throw new CloudServiceException(ex, "Specified file could not be loaded.");
            }
        }

        /// <summary>
        /// Uploads a file to the cloud service and assiciates it with the meta data provided. To use upload
        /// feature make sure configuration provider has AmazonBucket specified.
        /// </summary>
        /// <param name="file">Stream to the content that is to be uploaded.</param>
        /// <param name="newFileName">Name of the file on cloud.</param>
        /// <param name="metaData">Any meta data you want to be associated with the file.</param>
        public void UploadFile(Stream file, string newFileName, NameValueCollection metaData)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new CloudServiceException("Error occured when uploading file. The stream provided is invalid.");
                }
                else
                {
                    if (file.CanSeek)
                        file.Seek(0, SeekOrigin.Begin);
                }

                if (string.IsNullOrEmpty(newFileName))
                {
                    throw new CloudServiceException("Error occured when uploading file. New file key must be provided.");
                }

                using (AmazonS3 s3 = AWSClientFactory.CreateAmazonS3Client(this.cloudServiceConfigProvider.AWSAccessKeyId, this.cloudServiceConfigProvider.AWSSecretKey))
                {
                    TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest()
                        .WithBucketName(this.cloudServiceConfigProvider.AmazonBucket)
                        .WithAutoCloseStream(true)
                        .WithMetadata(metaData)
                        .WithKey(newFileName);

                    uploadRequest.InputStream = file;

                    using (TransferUtility transferUtility = new TransferUtility(s3))
                    {
                        transferUtility.Upload(uploadRequest);
                    }
                }
            }
            catch (AmazonS3Exception s3Exception)
            {
                throw new CloudServiceException(s3Exception, "Error occured when uploading file.");
            }
            catch (CloudServiceException apex)
            {
                throw apex;
            }
            catch (Exception ex)
            {
                throw new CloudServiceException(ex, "Error occured when uploading file.");
            }
        }

        #endregion

        #region Bulk Emails

        /// <summary>
        /// Method to send email through cloud service.
        /// If isHtml is true the body is interpreted as HTML content.
        /// </summary>
        public bool SendBulkEmail(string subject, string bodyText, string fromAddress, List<string> toAddresses, List<string> ccAddresses, List<string> replyToAddresses, bool isHtml)
        {
            try
            {
                if (replyToAddresses == null || replyToAddresses.Count == 0)
                {
                    replyToAddresses = new List<string>();
                    replyToAddresses.Add(fromAddress);
                }

                Message message = new Message();
                message.Body = new Body();

                Content bodyContent = new Content();
                bodyContent.Charset = "UTF-8";
                bodyContent.Data = bodyText;

                if (isHtml)
                    message.Body.Html = bodyContent;
                else
                    message.Body.Text = bodyContent;

                Content subjectContent = new Content();
                subjectContent.Charset = "UTF-8";
                subjectContent.Data = subject;

                message.Subject = subjectContent;

                Destination destination = new Destination(toAddresses);
                if (ccAddresses != null && ccAddresses.Count > 0)
                    destination.CcAddresses = ccAddresses;

                SendEmailRequest sendEmailRequest = new SendEmailRequest()
                    .WithDestination(destination)
                    .WithMessage(message)
                    .WithReplyToAddresses(replyToAddresses)
                    .WithSource(fromAddress);

                

                //Create AWS Client
                AmazonSimpleEmailService amazonSes = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(this.cloudServiceConfigProvider.AWSAccessKeyId, this.cloudServiceConfigProvider.AWSSecretKey);

                //Send email
                SendEmailResponse sendEmailResponse = amazonSes.SendEmail(sendEmailRequest);

                return true;
            }
            catch (Exception)
            {
                ///TODO: Write log
                return false;
            }
        }
        /// <summary>
        /// Method to send email through cloud service.
        /// If isHtml is true the body is interpreted as HTML content.
        /// </summary>
        public bool SendBulkEmail(string subject, string bodyText, string fromAddress, List<string> toAddresses, List<string> replyToAddresses, bool isHtml)
        {
            return SendBulkEmail(subject, bodyText, fromAddress, toAddresses, new List<string>(), replyToAddresses, isHtml);
        }

        /// <summary>
        /// Method to send email through cloud service. Please make sure that the attachements are of supported types.
        /// If MailMessage has LinkedResources associated with alternate views, they will be removed. Amazon SES currently
        /// requires content disposition for linked resources, which .Net does not support.
        /// </summary>
        /// <param name="mailMessage">.Net MailMessage</param>        
        public bool SendBulkEmail(MailMessage mailMessage)
        {
            //Create a temp unique folder under WorkingTempFolder
            string currentWorkingFolder = Path.Combine(this.cloudServiceConfigProvider.WorkingTempFolder, System.Guid.NewGuid().ToString("N"));

            try
            {
                if (mailMessage == null)
                    throw new CloudServiceException("Mail message must be provided.");

                if (mailMessage.To.Count == 0)
                    throw new CloudServiceException("To address is required.");

                if (mailMessage.From == null || string.IsNullOrEmpty(mailMessage.From.Address))
                    throw new CloudServiceException("From address is required.");

                if (!Directory.Exists(this.cloudServiceConfigProvider.WorkingTempFolder))
                    throw new CloudServiceConfigException("WorkingTempFolder specified does not exist.");

                DirectoryInfo dInfo = Directory.CreateDirectory(currentWorkingFolder);

                //Clear linked resources (Amazon has a glitch by requiring content-disposition for LinkedResource)
                foreach (AlternateView av in mailMessage.AlternateViews)
                {
                    av.LinkedResources.Clear();
                }

                //Setup content disposition for attachments
                //Amazon needs content disposition
                foreach (Attachment attachment in mailMessage.Attachments)
                {
                    if (attachment.ContentDisposition != null)
                    {
                        attachment.ContentDisposition.Inline = false;

                        if (attachment.ContentStream != null)
                            attachment.ContentDisposition.Size = attachment.ContentStream.Length;
                    }
                }

                //Create smpt client
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = currentWorkingFolder;
                
                client.Send(mailMessage);

                //Create AWS Client
                AmazonSimpleEmailService amazonSes = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(this.cloudServiceConfigProvider.AWSAccessKeyId, this.cloudServiceConfigProvider.AWSSecretKey);

                //At this point the email is created in the working folder. Ready one at a time and send to amazon
                foreach (FileInfo fInfo in dInfo.GetFiles())
                {
                    byte[] emailRawContent = File.ReadAllBytes(fInfo.FullName);
                    using (MemoryStream msEmail = new MemoryStream(emailRawContent, 0, emailRawContent.Length))
                    {
                        RawMessage rawMessage = new RawMessage(msEmail);

                        SendRawEmailRequest sendRawEmailRequest = new SendRawEmailRequest(rawMessage);
                        SendRawEmailResponse sendRawEmailRes = amazonSes.SendRawEmail(sendRawEmailRequest);
                    }
                }

                return true;
            }
            catch (CloudServiceException aex)
            {
                throw aex;
            }
            catch (Exception ex)
            {
                throw new CloudServiceException(ex, "Error occured when sending bulk email.");
            }
            finally
            {
                if(!this.cloudServiceConfigProvider.IsTraceMode == !string.IsNullOrEmpty(currentWorkingFolder))
                {
                    if (Directory.Exists(currentWorkingFolder))
                    {
                        foreach (string file in Directory.GetFiles(currentWorkingFolder))
                        {
                            try
                            {
                                if (File.Exists(file))
                                    File.Delete(file);
                            }
                            catch 
                            {
                                ///TODO: Add log once moved to common library
                            }
                        }

                        try
                        {
                            Directory.Delete(currentWorkingFolder);
                        }
                        catch
                        {
                            ///TODO: Add log once moved to common library
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to "Verify" a list of email addresses. Every email has to be verified before it is allowed to send email through cloud service.
        /// </summary>
        public void RegisterEmailAddress(List<string> emailAddresses)
        {
            try
            {
                if (emailAddresses == null || emailAddresses.Count == 0)
                    throw new CloudServiceException("Email address must be provided");

                //Create AWS Client
                AmazonSimpleEmailService amazonSes = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(this.cloudServiceConfigProvider.AWSAccessKeyId, this.cloudServiceConfigProvider.AWSSecretKey);

                foreach (string email in emailAddresses)
                {
                    VerifyEmailAddressRequest verifyEmailAddressReq = new VerifyEmailAddressRequest()
                        .WithEmailAddress(email);

                    VerifyEmailAddressResponse verifyEmailAddressRes = amazonSes.VerifyEmailAddress(verifyEmailAddressReq);
                }
            }
            catch (CloudServiceException aex)
            {
                throw aex;
            }
            catch (Exception ex)
            {
                throw new CloudServiceException(ex, "Error occured when trying to verify one of the emails");
            }
        }

        /// <summary>
        /// Method to "Verify" an email address.
        /// Every email has to be verified before it is allowed to send email through cloud service.
        /// </summary>        
        public void RegisterEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new CloudServiceException("Email address provided is invalid");
            }

            List<string> emails = new List<string>();
            emails.Add(emailAddress);

            RegisterEmailAddress(emails);
        }

        /// <summary>
        /// Method to get all "verified" email address associated with this account.
        /// </summary>       
        public List<string> ListRegisteredEmailAddresses()
        {
            try
            {
                //Create AWS Client
                AmazonSimpleEmailService amazonSes = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(this.cloudServiceConfigProvider.AWSAccessKeyId, this.cloudServiceConfigProvider.AWSSecretKey);

                ListVerifiedEmailAddressesRequest listVerEmailAddressesReq = new ListVerifiedEmailAddressesRequest();
                ListVerifiedEmailAddressesResponse listVerEmailAddressesRes = amazonSes.ListVerifiedEmailAddresses(listVerEmailAddressesReq);
                List<string> registeredEmailAddresses = new List<string>();

                if (listVerEmailAddressesRes.ListVerifiedEmailAddressesResult != null && listVerEmailAddressesRes.ListVerifiedEmailAddressesResult.VerifiedEmailAddresses != null)
                    registeredEmailAddresses = listVerEmailAddressesRes.ListVerifiedEmailAddressesResult.VerifiedEmailAddresses;

                return registeredEmailAddresses;
            }
            catch (CloudServiceException aex)
            {
                throw aex;
            }
            catch (Exception ex)
            {
                throw new CloudServiceException(ex, "Error occured when trying to verify one of the emails");
            }
        }

        #endregion
    }
}

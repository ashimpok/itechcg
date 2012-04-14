using System;

namespace ITechcg.Infrastructure.CloudServices
{
    /// <summary>
    /// Interface needs to be implmented by users of Clound interface to provide basic setting
    /// information.
    /// </summary>
    /// <remarks>
    /// Use DefaultCloudServiceConfigProvider if your settings are stored in web.config files
    /// are have the same keys as neeed by  
    /// <see cref="Itechcg.Infrastructure.CloudServices.CloudServiceConfigProviderBase"/>
    /// </remarks>
    public interface ICloudServiceConfigProvider
    {
        /// <summary>
        /// Gets the Access Key that Amazon provides
        /// </summary>
        string AWSAccessKeyId { get; }

        /// <summary>
        /// Gets the Secret Key (Password)
        /// </summary>
        string AWSSecretKey { get; }

        /// <summary>
        /// Gets the bucket associated with this provider. This is where the S3 files will be uploaded to.
        /// </summary>
        string AmazonBucket { get; }

        /// <summary>
        /// Gets number of days after which the Url retrieved for a file (object) from S3 will expire.
        /// </summary>
        int? DefaultExpirationDaysForUrl { get; }

        /// <summary>
        /// Gets the rooted path to a temporary working folder. This is used to drop working files for CloudServiceClient.
        /// </summary>
        string WorkingTempFolder { get; }

        /// <summary>
        /// Gets the value indicating whether to run the client in trace mode. In trace mode, for example, 
        /// temporary files generated will not be deleted. This is useful to debug an issue.
        /// </summary>
        bool IsTraceMode { get; }
    }
}

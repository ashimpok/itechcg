using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using ITechcg.Infrastructure.CloudServices.Exceptions;

namespace ITechcg.Infrastructure.CloudServices
{
    /// <summary>
    /// Default implementation of ICloudServiceConfigProvider where configs are read from
    /// following key
    ///     <list>
    ///         <item>AWSAccessKeyId</item>
    ///         <item>AWSSecretKey</item>
    ///         <item>AWSAmazonBucket</item>
    ///         <item>AWSDefaultExpirationDaysForUrl</item>
    ///         <item>AWSWorkingTempFolder</item>
    ///         <item>AWSIsTraceMode</item>
    ///     </list>
    /// </summary>
    public class CloudServiceConfigProviderBase : ICloudServiceConfigProvider
    {
        #region ICloudServiceConfigProvider Members

        public virtual string AWSAccessKeyId
        {
            get 
            {
                string value = GetKeyValue("AWSAccessKeyId");
                if (value == null)
                    throw new CloudServiceConfigException("AWSAccessKeyId configuration is missing");
                return value;
            }
        }

        public virtual string AWSSecretKey
        {
            get 
            {
                string value = GetKeyValue("AWSSecretKey");
                if (value == null)
                    throw new CloudServiceConfigException("AWSSecretKey configuration is missing");
                return value;
            }
        }

        public virtual string AmazonBucket
        {
            get 
            {
                string value = GetKeyValue("AWSAmazonBucket");
                return value;
            }
        }


        /// <summary>
        /// Gets the default expiration days for the document url. Default is set to 1 day.
        /// </summary>
        public virtual int? DefaultExpirationDaysForUrl
        {
            get 
            {
                string value = GetKeyValue("AWSDefaultExpirationDaysForUrl");
                int val;
                bool ret = int.TryParse(value, out val);
                if (ret)
                    return val;
                else
                    return 1;                
            }
        }

        public virtual string WorkingTempFolder
        {
            get 
            {
                string value = GetKeyValue("AWSWorkingTempFolder");
                
                if (value == null)
                    throw new CloudServiceConfigException("WorkingTempFolder configuration is missing");

                return value;
            }
        }
        
        public virtual bool IsTraceMode
        {
            get 
            {
                string value = GetKeyValue("AWSIsTraceMode");
                
                bool result;

                bool ret = bool.TryParse(value, out result);
                if(ret)
                    return result;

                return false;
            }
        }

        #endregion

        private string GetKeyValue(string key)
        {
            try
            {
                string val = ConfigurationManager.AppSettings[key];
                if (val != null && !string.IsNullOrEmpty(val.Trim()))
                    return val;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
    }
}

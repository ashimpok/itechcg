using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITechcg.Infrastructure.CloudServices;

namespace ITechcg.Infrastructure.CloudServices
{
    public class CloudServiceFactory
    {
        /// <summary>
        /// Factory method to create Amazon Client
        /// </summary>
        /// <param name="cloudServiceConfigProvider">Cloud service configuration provider. Use the default Create method if you want to use web.config based provider and where all the keys specified in DefaultCloudServiceConfigProvider exactly matches the key name you have in the config file.</param>
        public static CloudServiceClient CreateCloudServiceClient(ICloudServiceConfigProvider cloudServiceConfigProvider)
        {
            CloudServiceClient client = new CloudServiceClient(cloudServiceConfigProvider);            
            return client;
        }

        /// <summary>
        /// Factory method to create Amazon Client with default configuration provider.
        /// Use this if you want to use web.config based provider and where all the keys specified in DefaultCloudServiceConfigProvider exactly matches the key name you have in the config file.
        /// </summary>        
        public static CloudServiceClient CreateCloudServiceClient()
        {
            ICloudServiceConfigProvider cloudServiceConfigProvider = new CloudServiceConfigProviderBase();
            CloudServiceClient client = new CloudServiceClient(cloudServiceConfigProvider);
            return client;
        }
    }
}

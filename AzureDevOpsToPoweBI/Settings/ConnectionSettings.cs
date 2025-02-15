using Microsoft.VisualBasic;
using System.Configuration;

namespace AzureDevOpsToPowerBI.Settings.General
{
    public class ConnectionSettings : ConfigurationElement
    {
        /// <summary>
        /// The azuredevops uri.
        /// </summary>
        [ConfigurationProperty("TfsUri", IsRequired = true)]
        public string TfsUri
        {
            get => (string)this["TfsUri"];
        }

        /// <summary>
        /// The azuredevop Personal Access Token
        /// </summary>
        [ConfigurationProperty("PersonalAccessToken", IsRequired = true)]
        public string PersonalAccessToken
        {
            get => (string)this["PersonalAccessToken"];
        }

        /// <summary>
        /// The Azure Storage Connection String.
        /// </summary>
        [ConfigurationProperty("AzureStorageConnectionString", IsRequired = true)]
        public string AzureStorageConnectionString
        {
            get => (string)this["AzureStorageConnectionString"];
        }

        /// <summary>
        /// Work Item synchronization date.
        /// </summary>
        [ConfigurationProperty("WorkItemSyncDate", IsRequired = true)]
        public string WorkItemSyncDate
        {
            get => (string)this["WorkItemSyncDate"];
        }

    }
}
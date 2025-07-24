using Microsoft.VisualBasic;
using System.Configuration;

namespace AzureDevOpsToPowerBI.Settings.General
{
    /// <summary>
    /// Class to haldle the Connection section.
    /// </summary>
    public class FilterTags : ConfigurationElement
    {
        /// <summary>
        /// Tags to filer with.
        /// </summary>
        [ConfigurationProperty("Tags", IsRequired = true)]
        public string Tags
        {
            get => (string)this["Tags"];
        }

    }
}
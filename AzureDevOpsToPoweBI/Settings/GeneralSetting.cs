
using System.Configuration;
using AzureDevOpsToPowerBI.Settings.General;

namespace AzureDevOpsToPowerBI.Settings
{
    /// <summary>
    /// General Connection settings.
    /// </summary>
    public class GeneralSetting : ConfigurationSection
    {
        [ConfigurationProperty("Connection")]
        public ConnectionSettings Connection => (ConnectionSettings)this["Connection"];

        [ConfigurationProperty("FilterTag")]
        public FilterTags FilterTag => (FilterTags)this["FilterTag"];
    }
}
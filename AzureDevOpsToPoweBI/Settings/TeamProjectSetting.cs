
using System.Configuration;
using AzureDevOpsToPoweBI.Settings.TeamAndProjectSettings;

namespace AzureDevOpsToPowerBI.Settings
{
    /// <summary>
    /// Project settings.
    /// </summary>
    public class TeamProjectSetting : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ProjectCollection), AddItemName = "Project")]
        public ProjectCollection Projects => (ProjectCollection)this[""];
    }
}
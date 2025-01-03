using System.Configuration;

namespace AzureDevOpsToPoweBI.Settings.TeamAndProjectSettings
{
    public class TeamProjectSettings : ConfigurationElement
    {
        /// <summary>
        /// The azure devops Project Name.
        /// </summary>
        [ConfigurationProperty("ProjectName", IsRequired = true)]
        public string ProjectName
        {
            get => (string)this["ProjectName"];
        }

        /// <summary>
        /// The azuredevop project Area Path.
        /// </summary>
        [ConfigurationProperty("AreaPath", IsRequired = true)]
        public string AreaPath
        {
            get => (string)this["AreaPath"];
        }

        /// <summary>
        /// The TeamName in azure devops.
        /// </summary>
        [ConfigurationProperty("TeamName", IsRequired = true)]
        public string TeamName
        {
            get => (string)this["TeamName"];
        }
    }
}
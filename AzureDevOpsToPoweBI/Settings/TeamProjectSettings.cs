using System.Configuration;

namespace AzureDevOpsToPoweBI.Settings.TeamAndProjectSettings
{
    /// <summary>
    /// Class to handle with the project settings. 
    /// </summary>
    public class TeamProjectSettings : ConfigurationElement
    {
        /// <summary>
        /// The azure devops Project Name.
        /// </summary>
        [ConfigurationProperty("ProjectKey", IsRequired = true, IsKey = true)]
        public string ProjectKey
        {
            get => (string)this["ProjectKey"];
        }

        /// <summary>
        /// The azure devops Project Name.
        /// </summary>
        [ConfigurationProperty("ProjectName", IsRequired = true, IsKey = false)]
        public string ProjectName
        {
            get => (string)this["ProjectName"];
        }

        /// <summary>
        /// The azuredevop project Area Path.
        /// </summary>
        [ConfigurationProperty("AreaPath", IsRequired = true, IsKey = false)]
        public string AreaPath
        {
            get => (string)this["AreaPath"];
        }

        /// <summary>
        /// The TeamName in azure devops.
        /// </summary>
        [ConfigurationProperty("TeamName", IsRequired = true, IsKey = false)]
        public string TeamName
        {
            get => (string)this["TeamName"];
        }

        /// <summary>
        /// The level of the iteration where the name of the project/produt is. This is used to filer the iterations by project/product.
        /// </summary>
        [ConfigurationProperty("IterationLevel", IsRequired = true, IsKey = false)]
        public int IterationLevel
        {
            get => (int)this["IterationLevel"];
        }
    }
}
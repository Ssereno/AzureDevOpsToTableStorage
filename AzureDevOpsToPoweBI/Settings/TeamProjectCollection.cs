using System.Configuration;

namespace AzureDevOpsToPoweBI.Settings.TeamAndProjectSettings
{
    public class ProjectCollection : ConfigurationElementCollection, IEnumerable<TeamProjectSettings>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TeamProjectSettings();   
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TeamProjectSettings)element).ProjectKey;
        }

        public new IEnumerator<TeamProjectSettings> GetEnumerator()
        {
            foreach (var key in BaseGetAllKeys())
            {
                yield return (TeamProjectSettings)BaseGet(key);
            }
        }
    }
}

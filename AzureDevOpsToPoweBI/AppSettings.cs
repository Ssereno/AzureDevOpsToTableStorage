
namespace AzureDevOpsToPowerBI
{
    internal static class AppSettings
    {
        private static string _tfsUri;
        private static string _personalAccessToken;
        private static string _azureStorageConnectionString;
        private static string _workItemSyncDate;
        private static List<string> _tags;

        internal static string TfsUri
        {
            get { return _tfsUri; }
            set { _tfsUri = value; }
        }

        public static string PersonalAccessToken
        {
            get { return _personalAccessToken; }
            set { _personalAccessToken = value; }
        }

        public static string AzureStorageConnectionString
        {
            get { return _azureStorageConnectionString; }
            set { _azureStorageConnectionString = value; }
        }

        public static string WorkItemSyncDate
        {
            get { return _workItemSyncDate; }
            set { _workItemSyncDate = value; }
        }

        public static List<string> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }
    }
}
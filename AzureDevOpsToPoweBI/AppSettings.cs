
namespace AzureDevOpsToPowerBI

{
    internal static class AppSettings
    {
        private static string _tfsUri;
        private static string _personalAccessToken;
        private static string _azureStorageConnectionString;

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
    }
}
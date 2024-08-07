
namespace AzureDevOpsToPowerBI

{
    internal static class AppSettings 
    {
        internal static string TfsUri { 
            get {
                return "";
            }
        }

        internal static string PersonalAccessToken { 
            get {
                return "";
            }
        }

        internal static string AzureStorageConnectionString { 
            get {
                return "";
            }
        }

        internal static string UserStoriesTableName { 
            get {
                return "UserStories";
            }
        }           
        internal static string TasksTableName { 
            get {
                return "Tasks";
            }
        }
        internal static string AreasTableName { 
            get {
                return "Areas";
            }
        }
        internal static string IterationTableName { 
            get {
                return "Iteration";
            }
        }
    }
}
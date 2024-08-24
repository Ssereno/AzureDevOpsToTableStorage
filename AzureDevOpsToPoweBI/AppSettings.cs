
namespace AzureDevOpsToPowerBI

{
    internal static class AppSettings 
    {
        internal static string TfsUri { 
            get {
                return "";
            }
        }

        public static string PersonalAccessToken { 
            get {
                return "";
            }
        }

        public static string AzureStorageConnectionString { 
            get {
                return "";
            }
        }

        public static string UserStoriesTableName { 
            get {
                return "UserStories";
            }
        }           
        public static string TasksTableName { 
            get {
                return "Tasks";
            }
        }
        public static string AreasTableName { 
            get {
                return "Areas";
            }
        }
        public static string IterationTableName { 
            get {
                return "Iteration";
            }
        }

        public static string SprintCapacityTableName { 
            get {
                return "SprintCapacity";
            }
        }
    }
}
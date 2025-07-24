using AzureDevOpsToPowerBI.Settings;
using AzureDevOpsToPowerBI.Settings.General;
using System.Configuration;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Main Class.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            string asciiTitle = @"
                _                          ____              ___              ____                   
               / \    _____   _ _ __ ___  |  _ \  _____   __/ _ \ _ __  ___  / ___| _   _ _ __   ___ 
              / _ \  |_  / | | | '__/ _ \ | | | |/ _ \ \ / / | | | '_ \/ __| \___ \| | | | '_ \ / __|
             / ___ \  / /| |_| | | |  __/ | |_| |  __/\ V /| |_| | |_) \__ \  ___) | |_| | | | | (__ 
            /_/   \_\/___|\__,_|_|  \___| |____/ \___| \_/  \___/| .__/|___/ |____/ \__, |_| |_|\___|
                                                                 |_|                |___/                                       
            ";

            Console.WriteLine(asciiTitle);

            // Read settings.
            var connecionSettings = ConfigurationManager.GetSection("General") as GeneralSetting;

            AppSettings.AzureStorageConnectionString = connecionSettings.Connection.AzureStorageConnectionString;
            AppSettings.TfsUri = connecionSettings.Connection.TfsUri;
            AppSettings.PersonalAccessToken = connecionSettings.Connection.PersonalAccessToken;
            AppSettings.WorkItemSyncDate = connecionSettings.Connection.WorkItemSyncDate;
            AppSettings.Tags = ParseTags(connecionSettings.FilterTag.Tags);

            var teamProjectSettings = ConfigurationManager.GetSection("TeamProject") as TeamProjectSetting;

            try
            {
                // Syncronization mode.
                Console.WriteLine("Select the syncronization mode:");
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine($"1 - Update by Date (Delete work itens with create date greater then {AppSettings.WorkItemSyncDate} and sync again)");
                Console.WriteLine($"2 - Update By State (Delete work itens not in closed state and sync again).");
                Console.WriteLine($"3 - Full update (Delete all UserStory and Task tables and sync again).");
                Console.WriteLine("-----------------------------------------------");

                string option = Console.ReadLine();

                // Clean data before sync
                await DeleteDataBeforeSync( option );

                if (teamProjectSettings != null)
                {
                    foreach (var project in teamProjectSettings.Projects)
                    {
                        // Update by Date or Full update
                        if ((option == "1") || (option == "3"))
                        {
                            Console.Write($"User Stories for {project.ProjectKey}. \n");
                            var userStories = await UserStoriesManager.GetTfsUserStories(project.ProjectKey, project.ProjectName, project.AreaPath);
                            await AzureTablesManager<UserStory>.InsertIntoAzureTableBulkAsync(userStories, "UserStories");

                            Console.Write($"Tasks for {project.ProjectKey}. \n");
                            var tasks = await TaskManager.GetTfsTasks(project.ProjectKey, project.ProjectName, project.AreaPath);
                            await AzureTablesManager<TfsTasks>.InsertIntoAzureTableBulkAsync(tasks, "Tasks");

                            Console.Write($"Bugs for {project.ProjectKey}. \n");
                            var bugs = await BugManager.GetBugs(project.ProjectKey, project.ProjectName, project.AreaPath);
                            await AzureTablesManager<Bug>.InsertIntoAzureTableBulkAsync(bugs, "Bugs");
                        }
                        else if (option == "2")
                        {
                            Console.Write($"User Stories for {project.ProjectKey}. \n");
                            var userStories = await UserStoriesManager.GetTfsUserStories(project.ProjectKey, project.ProjectName, project.AreaPath);
                            await AzureTablesManager<UserStory>.UpsertIntoAzureTableAsync(userStories, "UserStories");

                            Console.Write($"Tasks for {project.ProjectKey}. \n");
                            var tasks = await TaskManager.GetTfsTasks(project.ProjectKey, project.ProjectName, project.AreaPath);
                            await AzureTablesManager<TfsTasks>.UpsertIntoAzureTableAsync(tasks, "Tasks");

                            Console.Write($"Bugs for {project.ProjectKey}. \n");
                            var bugs = await BugManager.GetBugs(project.ProjectKey, project.ProjectName, project.AreaPath);
                            await AzureTablesManager<Bug>.UpsertIntoAzureTableAsync(bugs, "Bugs");
                        }

                        // Sync areas
                        Console.Write($"Area for {project.ProjectKey}. \n");
                        var areas = await AreaManager.GetAreas(project.ProjectKey, project.ProjectName, project.AreaPath);
                        await AzureTablesManager<Area>.UpsertIntoAzureTableAsync(areas, "Areas");

                        // Sync iterations
                        Console.Write($"Iterations for {project.ProjectKey}. \n");
                        var iterations = await IterationManager.GetTfsIterations(project.ProjectKey, project.ProjectName, project.TeamName, project.IterationLevel);
                        await AzureTablesManager<Iteration>.UpsertIntoAzureTableAsync(iterations, "Iteration");

                        // Sync Sprint Capacity
                        Console.Write($"Sprint Capacity for {project.ProjectKey}. \n");
                        var SprintCapacity = await SprintCapacityManager.GetCapacityAsync(project.ProjectKey, project.ProjectName, project.TeamName);
                        await AzureTablesManager<SprintCapacity>.UpsertIntoAzureTableAsync(SprintCapacity, "SprintCapacity");

                        Console.Write($"{project.ProjectKey} Done. \n");
                    }

                    Console.Write("All Done......");
                    Console.ReadLine();
                }
                else
                {
                    Console.Write("No projects configured. Please add a project in app.config");
                    Console.ReadLine();
                }

                // Just to creat the table with dummy data.
                await CreateProjectProfile();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(e.Message.ToString());
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Delete data before the syncronization.
        /// </summary>
        /// <param name="option">The syncronization mode.</param>
        /// <returns></returns>
        private static  async Task DeleteDataBeforeSync(string option)
        {
            try
            {
                Console.Write("Clean data before sync. \n");

                if (option == "1")
                {
                    if (await AzureTablesManager<UserStory>.TableExistsAsync("UserStories"))
                        await AzureTablesManager<UserStory>.DeleteAllEntitieswithWithFilterAsync("UserStories", $"CreatedDate ge '{AppSettings.WorkItemSyncDate}'");

                    if (await AzureTablesManager<TfsTasks>.TableExistsAsync("Tasks"))
                        await AzureTablesManager<TfsTasks>.DeleteAllEntitieswithWithFilterAsync("Tasks", $"CreatedDate ge '{AppSettings.WorkItemSyncDate}'");

                    if (await AzureTablesManager<TfsTasks>.TableExistsAsync("Bugs"))
                        await AzureTablesManager<TfsTasks>.DeleteAllEntitieswithWithFilterAsync("Bugs", $"CreatedDate ge '{AppSettings.WorkItemSyncDate}'");
                }
                else if (option == "2")
                {
                    if (await AzureTablesManager<UserStory>.TableExistsAsync("UserStories"))
                        await AzureTablesManager<UserStory>.DeleteAllEntitieswithWithFilterAsync("UserStories", "State ne 'Closed'");

                    if (await AzureTablesManager<TfsTasks>.TableExistsAsync("Tasks"))
                        await AzureTablesManager<TfsTasks>.DeleteAllEntitieswithWithFilterAsync("Tasks", "State ne 'Closed'");

                    if (await AzureTablesManager<TfsTasks>.TableExistsAsync("Bugs"))
                        await AzureTablesManager<TfsTasks>.DeleteAllEntitieswithWithFilterAsync("Bugs", "State ne 'Closed'");
                }
                else if (option == "3")
                {
                    if (await AzureTablesManager<UserStory>.TableExistsAsync("UserStories"))
                        await AzureTablesManager<UserStory>.DeleteAllEntitiesAsync("UserStories");

                    if (await AzureTablesManager<TfsTasks>.TableExistsAsync("Tasks"))
                        await AzureTablesManager<TfsTasks>.DeleteAllEntitiesAsync("Tasks");

                    if (await AzureTablesManager<TfsTasks>.TableExistsAsync("Bugs"))
                        await AzureTablesManager<TfsTasks>.DeleteAllEntitiesAsync("Bugs");
                }

                Console.Write($"Data cleaning done. Prepare to get new data greater or equal to: {AppSettings.WorkItemSyncDate}. \n");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString(), ex);
            }
        }

        /// <summary>
        /// Create a empty project profile table and add dummy data.
        /// This table is optional and is fed by hand.
        /// </summary>
        /// <returns></returns>
        private static async Task CreateProjectProfile()
        {
            var projectProfiles = new List<ProjectProfile>();

            projectProfiles.Add(new ProjectProfile{
                AreaPath ="",
                AreaSK = "",
                IterationName ="",
                IterationSK ="",
                Velocity = 0,
                ProjectEffort = 0,
                ProjectEndDate = DateTime.Now.ToString(),
                ProjectStartDate = DateTime.Now.ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey= Guid.NewGuid().ToString()
            });

            try
            {
                // Create if not exist.
                if (!await AzureTablesManager<ProjectProfile>.TableExistsAsync("ProjectProfile"))
                    await AzureTablesManager<ProjectProfile>.InsertIntoAzureTableAsync(projectProfiles, "ProjectProfile");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString(), ex);
            }
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagString"></param>
        /// <returns></returns>
        private static List<string> ParseTags(string tagString)
        {
            if (string.IsNullOrWhiteSpace(tagString))
                return new List<string>();

            return tagString
                .Split(';')
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrEmpty(tag))
                .ToList();
        }
    }
}

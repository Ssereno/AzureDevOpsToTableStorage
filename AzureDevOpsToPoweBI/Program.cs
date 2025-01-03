using System.Configuration;
using AzureDevOpsToPowerBI.Settings;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Main Class.
    /// </summary>
    class Program
    {
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

            var teamProjectSettings = ConfigurationManager.GetSection("TeamProject") as TeamProjectSetting;

            try
            {
                // Syncronization mode.
                Console.WriteLine("Select the syncronization mode:");
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("1 - Full Mode (Delete all Data).");
                Console.WriteLine("2 - Update Mode (Update existent Data).");
                Console.WriteLine("-----------------------------------------------");

                string option = Console.ReadLine();

                Console.Write("Clean data before sync. \n");

                if (option == "1")
                {
                    if (await AzureTablesManager<UserStory>.TableExistsAsync("UserStories"))
                        await AzureTablesManager<UserStory>.DeleteAllEntitiesAsync("UserStories");

                    if (await AzureTablesManager<TfsTasks>.TableExistsAsync("Tasks"))
                        await AzureTablesManager<TfsTasks>.DeleteAllEntitiesAsync("Tasks");
                }
                else if (option == "2")
                {
                    if (await AzureTablesManager<UserStory>.TableExistsAsync("UserStories"))
                        await AzureTablesManager<UserStory>.DeleteAllEntitieswithWithFilterAsync("UserStories", "State ne 'Closed'");

                    if (await AzureTablesManager<TfsTasks>.TableExistsAsync("Tasks"))
                        await AzureTablesManager<TfsTasks>.DeleteAllEntitieswithWithFilterAsync("Tasks", "State ne 'Closed'");
                }

                Console.Write("Data cleaning done. Prepare to get new data. \n");

                if (teamProjectSettings != null)
                {
                    foreach (var project in teamProjectSettings.Projects)
                    {
                        if (option == "1")
                        {
                            Console.Write($"User Stories for {project.ProjectName}. \n");
                            var userStories = await UserStoriesManager.GetTfsUserStories(project.ProjectName, project.AreaPath);
                            await AzureTablesManager<UserStory>.InsertIntoAzureTableBulkAsync(userStories, "UserStories");

                            Console.Write($"Tasks for {project.ProjectName}. \n");
                            var tasks = await TaskManager.GetTfsTasks(project.ProjectName, project.AreaPath);
                            await AzureTablesManager<TfsTasks>.InsertIntoAzureTableBulkAsync(tasks, "Tasks");
                        }
                        else if (option == "2")
                        {
                            Console.Write($"User Stories for {project.ProjectName}. \n");
                            var userStories = await UserStoriesManager.GetTfsUserStories(project.ProjectName, project.AreaPath);
                            await AzureTablesManager<UserStory>.UpsertIntoAzureTableAsync(userStories, "UserStories");

                            Console.Write($"Tasks for {project.ProjectName}. \n");
                            var tasks = await TaskManager.GetTfsTasks(project.ProjectName, project.AreaPath);
                            await AzureTablesManager<TfsTasks>.UpsertIntoAzureTableAsync(tasks, "Tasks");
                        }

                        Console.Write($"Area for {project.ProjectName}. \n");
                        var areas = await AreaManager.GetAreas(project.ProjectName, project.AreaPath);
                        await AzureTablesManager<Area>.UpsertIntoAzureTableAsync(areas, "Areas");

                        Console.Write($"Iterations for {project.ProjectName}. \n");
                        var iterations = await IterationManager.GetTfsIterations(project.ProjectName, project.TeamName);
                        await AzureTablesManager<Iteration>.UpsertIntoAzureTableAsync(iterations, "Iteration");

                        Console.Write($"Sprint Capacity for {project.ProjectName}. \n");
                        var SprintCapacity = await SprintCapacityManager.GetCapacityAsync(project.ProjectName, project.TeamName);
                        await AzureTablesManager<SprintCapacity>.UpsertIntoAzureTableAsync(SprintCapacity, "SprintCapacity");

                        Console.Write($"{project.ProjectName} Done. \n");
                    }

                    Console.Write("All Done......");
                    Console.ReadLine();
                }
                else
                {
                    Console.Write("No projects configured. Please add a project in app.config");
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());
                Console.ReadLine();
            }
        }
    }
}

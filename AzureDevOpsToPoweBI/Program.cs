using System;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Main Class.
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Start Sincronization....");

            // List of projects to get data.
            // Project Name; Area Path, Team Name
            var appsProjects = new List<Tuple<string,string,string>>{
                    Tuple.Create("ProjectA","ProjectA\\ProjectA Team","ProjectA Team")
            };
           
            Console.Write("Clean data before sync.");

            try
            {
                if (await AzureTablesManager<UserStory>.TableExistsAsync(AppSettings.UserStoriesTableName))  
                await AzureTablesManager<UserStory>.DeleteAllEntitiesAsync(AppSettings.UserStoriesTableName);

                if (await AzureTablesManager<TfsTasks>.TableExistsAsync(AppSettings.TasksTableName)) 
                    await AzureTablesManager<TfsTasks>.DeleteAllEntitiesAsync(AppSettings.TasksTableName);

                Console.Write("Data cleansing done. Prepare to get new data.");

                foreach(Tuple<string, string,string> tuple in appsProjects)
                {
                    var userStories = await UserStoriesManager.GetTfsUserStories(tuple.Item1, tuple.Item2);
                    await AzureTablesManager<UserStory>.InsertIntoAzureTableBulkAsync(userStories,AppSettings.UserStoriesTableName);
                    Console.Write($"User Stories for {tuple.Item1} Done. \n");

                    var tasks = await TaskManager.GetTfsTasks(tuple.Item1, tuple.Item2);
                    await AzureTablesManager<TfsTasks>.InsertIntoAzureTableBulkAsync(tasks,AppSettings.TasksTableName);
                    Console.Write($"Tasks for {tuple.Item1} Done. \n");

                    var areas = await AreaManager.GetAreas(tuple.Item1, tuple.Item2);
                    await AzureTablesManager<Area>.UpsertIntoAzureTableAsync(areas,AppSettings.AreasTableName);
                    Console.Write($"Area for {tuple.Item1} Done. \n");
                    
                    var iterations = await IterationManager.GetTfsIterations(tuple.Item1,tuple.Item3);
                    await AzureTablesManager<Iteration>.UpsertIntoAzureTableAsync(iterations,AppSettings.IterationTableName);
                    Console.Write($"Iterations for {tuple.Item1} Done. \n");

                    var SprintCapacity = await SprintCapacityManager.GetCapacityAsync(tuple.Item1,tuple.Item3);
                    await AzureTablesManager<SprintCapacity>.UpsertIntoAzureTableAsync(SprintCapacity,AppSettings.SprintCapacityTableName);
                    Console.Write($"Sprint Capacity for {tuple.Item1} Done. \n");
                }

                Console.Write("All Done.");
            }
            catch(Exception e)
            {
                Console.Write(e.Message.ToString());
            }
        }
    }
}

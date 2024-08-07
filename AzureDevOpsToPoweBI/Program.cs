using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Data.OData;
using Azure;
using Azure.Data.Tables;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace AzureDevOpsToPowerBI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Start Sincronization....");

            var appsProjects = new List<Tuple<string,string,string>>{
                    Tuple.Create("FMM","FMM\\FMM Team","FMM Team"),
                    Tuple.Create("ResourceMonitor","ResourceMonitor\\Resource Monitor App","Resource Monitor App"),
                    Tuple.Create("WIPAnalyzer","WIPAnalyzer\\WIPAnalyzer App","WIPAnalyzer Team"),
                    Tuple.Create("ASM-APM","ASM-APM\\SMTA Team","SMTA Team"),
                    Tuple.Create("IndustryTemplates","IndustryTemplates","Industry Templates")
            };

            Console.Write("Clean data before sync.");

            if (await AzureTablesManager<UserStory>.TableExistsAsync(AppSettings.UserStoriesTableName))  
                await AzureTablesManager<UserStory>.DeleteAllEntitiesAsync(AppSettings.UserStoriesTableName);

            if (await AzureTablesManager<TfsTasks>.TableExistsAsync(AppSettings.TasksTableName)) 
                await AzureTablesManager<TfsTasks>.DeleteAllEntitiesAsync(AppSettings.TasksTableName);

            if (await AzureTablesManager<Area>.TableExistsAsync(AppSettings.AreasTableName)) 
                await AzureTablesManager<Area>.DeleteAllEntitiesAsync(AppSettings.AreasTableName);
            
            if (await AzureTablesManager<Iteration>.TableExistsAsync(AppSettings.IterationTableName))
                await AzureTablesManager<Iteration>.DeleteAllEntitiesAsync(AppSettings.IterationTableName);

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
                await AzureTablesManager<Area>.InsertIntoAzureTableBulkAsync(areas,AppSettings.AreasTableName);
                Console.Write($"Area for {tuple.Item1} Done. \n");

                var iterations = await IterationManager.GetTfsIterations(tuple.Item1,tuple.Item3);
                await AzureTablesManager<Iteration>.InsertIntoAzureTableBulkAsync(iterations,AppSettings.IterationTableName);
                Console.Write($"Iterations for {tuple.Item1} Done. \n");
            }
        }
    }
}

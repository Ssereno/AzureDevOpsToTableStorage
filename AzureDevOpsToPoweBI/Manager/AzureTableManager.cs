using System.Reflection.Metadata;
using Azure;
using Azure.Data.Tables;

namespace AzureDevOpsToPowerBI
{
    internal static class AzureTablesManager<T> where T : class, ITableEntity
    {
        internal static async Task InsertIntoAzureTable(List<T> workItems, string tableName)
        {
            var tableClient = new TableClient(AppSettings.AzureStorageConnectionString, tableName);
            await tableClient.CreateIfNotExistsAsync();

            foreach (var workItem in workItems)
            {
                await tableClient.AddEntityAsync(workItem);
            }
        }

        internal static async Task InsertIntoAzureTableBulkAsync(List<T> workItems, string tableName)
        {
            var tableClient = new TableClient(AppSettings.AzureStorageConnectionString, tableName);
            await tableClient.CreateIfNotExistsAsync();

            int batchSize = 100; // Azure Table Storage allows a maximum of 100 entities per batch
            var tasks = new List<Task>();

            for (int i = 0; i < workItems.Count; i += batchSize)
            {
                var batch = workItems.GetRange(i, Math.Min(batchSize, workItems.Count - i));
                var transactionActions = new List<TableTransactionAction>();

                foreach (var entity in batch)
                {
                    transactionActions.Add(new TableTransactionAction(TableTransactionActionType.Add, entity));
                }

                // Add the batch insert task to the list of tasks
                tasks.Add(tableClient.SubmitTransactionAsync(transactionActions));
            }

            // Await all the tasks to complete
            await Task.WhenAll(tasks);
        }


        internal static async Task DeleteAllEntitiesAsync(string tableName)
        {
            var tableClient = new TableClient(AppSettings.AzureStorageConnectionString, tableName);

            await foreach (Page<TableEntity> page in tableClient.QueryAsync<TableEntity>().AsPages())
            {
                foreach (TableEntity entity in page.Values)
                {
                    await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
                }
            }
        }

        internal static async Task DeleteTableAsync(string tableName)
        {
            try
            {
                // Get a reference to the table
                var tableClient = new TableClient(AppSettings.AzureStorageConnectionString, tableName);

                // Delete the table
                await tableClient.DeleteAsync();

                Console.WriteLine($"Table '{tableName}' deleted successfully.");
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Failed to delete the table: {ex.Message}");
            }
        }

        internal static async Task<bool> TableExistsAsync(string tableName)
        {
            try
            {
                var tableServiceClient = new TableServiceClient(AppSettings.AzureStorageConnectionString);      
                await foreach(var tbl in tableServiceClient.QueryAsync(t => t.Name == tableName))
                {
                    return true;
                }

                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return false;
            }
        }
    }
}

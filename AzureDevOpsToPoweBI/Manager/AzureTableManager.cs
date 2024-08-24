using Azure;
using Azure.Data.Tables;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Manager to handle (add, update and delete) with operations on azure table.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class AzureTablesManager<T> where T : class, ITableEntity
    {
        /// <summary>
        /// Insert new data into a azure table.
        /// </summary>
        /// <param name="workItems">List of values to add.</param>
        /// <param name="tableName">Table name to create.</param>
        /// <returns></returns>
        internal static async Task InsertIntoAzureTableAsync(List<T> workItems, string tableName)
        {
            var tableClient = new TableClient(AppSettings.AzureStorageConnectionString, tableName);
            await tableClient.CreateIfNotExistsAsync();

            foreach (var workItem in workItems)
            {
                await tableClient.AddEntityAsync(workItem);
            }
        }

        /// <summary>
        /// Insert or update data on a azure table.
        /// </summary>
        /// <param name="workItems">List of values to add.</param>
        /// <param name="tableName">Table name to create.</param>
        /// <returns></returns>
        internal static async Task UpsertIntoAzureTableAsync(List<T> workItems, string tableName)
        {
            var tableClient = new TableClient(AppSettings.AzureStorageConnectionString, tableName);
            await tableClient.CreateIfNotExistsAsync();

            foreach (var workItem in workItems)
            {
                await tableClient.UpsertEntityAsync(workItem,TableUpdateMode.Merge);
            }
        }

        /// <summary>
        /// Insert data in bulk (100 itens) on a azure table.
        /// </summary>
        /// <param name="workItems">List of values to add.</param>
        /// <param name="tableName">Table name to create.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete the content of a azure table.
        /// </summary>
        /// <param name="tableName">Table name to delete data.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete a table.
        /// </summary>
        /// <param name="tableName">Table name to delete.</param>
        /// <returns></returns>
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

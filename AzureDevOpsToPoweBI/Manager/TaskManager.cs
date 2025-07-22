using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Manager to handle with the task entity.
    /// </summary>
    internal static class TaskManager
    {
        internal static async Task<List<TfsTasks>> GetTfsTasks(string projectkey, string projectname, string areapath)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{string.Empty}:{AppSettings.PersonalAccessToken}")));
                
            string tfsUri = string.Format($"{AppSettings.TfsUri}/{{0}}/_odata/v4.0-preview/WorkItems?$select=WorkItemId,Title,WorkItemType,State,AreaSK,IterationSK,TagNames,ParentWorkItemId,OriginalEstimate,CompletedWork,CompletedDate,CreatedDate,ClosedDate&$filter=WorkItemType eq 'Task' and startswith(Area/AreaPath,'{{1}}') and not contains(State, 'Removed') and CreatedDate ge {{2}} &$orderby=CreatedDate desc",projectname,areapath, AppSettings.WorkItemSyncDate);

            var response = await client.GetAsync(tfsUri);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var workItems = JsonSerializer.Deserialize<TaskWorkItemResponse>(responseBody);
            var tasks = new List<TfsTasks>();

            foreach (var item in workItems.Value)
            {
                tasks.Add(new TfsTasks
                {
                    PartitionKey = projectkey,
                    RowKey = item.WorkItemId.ToString(),
                    Title= item.Title,
                    State= item.State,
                    WorkItemId= item.WorkItemId,
                    AreaSK= item.AreaSK,
                    IterationSK= item.IterationSK,
                    TagNames= item.TagNames,
                    ParentWorkItemId = item.ParentWorkItemId,
                    OriginalEstimate = item.OriginalEstimate,
                    CompletedWork = item.CompletedWork,
                    CreatedDate = item.CreatedDate,
                    ClosedDate = item.ClosedDate
                });
            }

            return tasks;
        }
    }
    /// <summary>
    /// Allow access to the json response.
    /// </summary>
    internal class TaskWorkItemResponse
    {
        [JsonPropertyName("value")]
        public List<TfsTasks> Value { get; set; }
    }
}

using Azure.Data.Tables;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Manager to handle with the bug entity.
    /// </summary>
    internal static class BugManager
    {
        /// <summary>
        /// Get all the Bugs for the selected project.
        /// </summary>
        /// <param name="projectname">The project name.</param>
        /// <param name="areapath">The Bug or project iteration.</param>
        /// <returns></returns>
        internal static async Task<List<Bug>> GetBugs(string projectkey, string projectname, string areapath)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{string.Empty}:{AppSettings.PersonalAccessToken}")));
                
            string tfsUri = string.Format($"{AppSettings.TfsUri}/{{0}}/_odata/v4.0-preview/WorkItems?$Select=WorkItemId,Title,State,AreaSK,IterationSK,CreatedDate,ActivatedDate,ClosedDate,ResolvedDate,CompletedDate,Severity,Custom_TicketID,Custom_TicketPriority,Custom_BugType&$filter=WorkItemType eq 'Bug' and startswith(Area/AreaPath,'{{1}}') and CreatedDate ge {{2}}&$orderby=CreatedDate desc",projectname,areapath, AppSettings.WorkItemSyncDate);
            var response = await client.GetAsync(tfsUri);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var workItems = JsonSerializer.Deserialize<BugWorkItemResponse>(responseBody);
            var bugs = new List<Bug>();

            foreach (var item in workItems.Value)
            {
                bugs.Add(new Bug
                {
                    PartitionKey = projectkey,
                    RowKey = item.WorkItemId.ToString(),
                    AreaSK = item.AreaSK,
                    IterationSK = item.IterationSK,
                    WorkItemId = item.WorkItemId,
                    Title = item.Title,
                    State = item.State,
                    CreatedDate = item.CreatedDate,
                    Severity = item.Severity,
                    Custom_BugType = item.Custom_BugType,
                    Custom_TicketID = item.Custom_TicketID,
                    Custom_TicketPriority = item.Custom_TicketPriority,
                    ParentWorkItemId = item.ParentWorkItemId,
                    ActivatedDate = DateTimeHelper.GetEffectiveActivatedDate(item.CompletedDate, item.ActivatedDate),
                    ResolvedDate = DateTimeHelper.GetEffectiveResolutionDate(item.CompletedDate, item.ClosedDate, item.ResolvedDate),
                    ClosedDate = DateTimeHelper.GetEffectiveCompletionDate(item.CompletedDate, item.ClosedDate)
                });
            }

            return bugs;
        }
    }
    /// <summary>
    /// Allow access to the json response.
    /// </summary>
    internal class BugWorkItemResponse
    {
        [JsonPropertyName("value")]
        public List<Bug> Value { get; set; }
    }
}
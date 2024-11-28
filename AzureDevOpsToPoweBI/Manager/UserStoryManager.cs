using System.Net.Http.Headers;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace AzureDevOpsToPowerBI
{
    internal static class UserStoriesManager
    {
        internal static async Task<List<UserStory>> GetTfsUserStories(string projectname, string areapath)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{string.Empty}:{AppSettings.PersonalAccessToken}")));

            string tfsUri = string.Format($"{AppSettings.TfsUri}/{{0}}/_odata/v4.0-preview/WorkItems?$select=WorkItemId,Title,WorkItemType,State,StoryPoints,LeadTimeDays,CycleTimeDays,CreatedDate,ResolvedDate,AreaSK,IterationSK,ActivatedDate,ClosedDate,ParentWorkItemId, TagNames&$filter=WorkItemType eq 'User Story' and startswith(Area/AreaPath,'{{1}}') and not contains(State, 'Removed') and CreatedDate ge 2023-01-01T00:00:00Z &$orderby=CreatedDate desc",projectname,areapath);

            var response = await client.GetAsync(tfsUri);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var workItems = JsonSerializer.Deserialize<WorkItemResponse>(responseBody);
            var userStories = new List<UserStory>();

            foreach (var item in workItems.Value)
            {
                userStories.Add(new UserStory
                {
                    PartitionKey = projectname,
                    RowKey = item.WorkItemId.ToString(),
                    Title= item.Title,
                    State= item.State,
                    WorkItemId= item.WorkItemId,
                    StoryPoints= item.StoryPoints,
                    LeadTimeDays= item.LeadTimeDays,
                    CycleTimeDays= item.CycleTimeDays,
                    CreatedDate= item.CreatedDate,
                    ResolvedDate= item.ResolvedDate,
                    AreaSK= item.AreaSK,
                    IterationSK= item.IterationSK,
                    ActivatedDate= item.ActivatedDate,
                    ClosedDate = item.ClosedDate,
                    ParentWorkItemId = item.ParentWorkItemId,
                    TagNames = item.TagNames
                });
            }

            return userStories;
        }

        public class WorkItemResponse
        {
            [JsonPropertyName("value")]
            public List<UserStory> Value { get; set; }
        }
    }
}

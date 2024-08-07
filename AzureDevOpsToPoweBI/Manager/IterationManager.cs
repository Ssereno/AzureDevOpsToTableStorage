using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    internal static class IterationManager
    {
        internal static async Task<List<Iteration>> GetTfsIterations(string projectname, string teamName)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{string.Empty}:{AppSettings.PersonalAccessToken}")));
                
            string tfsUri = string.Format($"{AppSettings.TfsUri}/{{0}}/_odata/v4.0-preview/Iterations?$select=IterationName,IterationPath,IterationSK,StartDate,EndDate",projectname);

            var response = await client.GetAsync(tfsUri);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var workItems = JsonSerializer.Deserialize<WorkItemResponse>(responseBody);
            var iteration = new List<Iteration>();

            foreach (var item in workItems.Value)
            {
                iteration.Add(new Iteration
                {
                    PartitionKey = projectname,
                    RowKey = Guid.NewGuid().ToString(),
                    IterationName = item.IterationName,
                    IterationPath = item.IterationPath,
                    IterationSK = item.IterationSK,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    SprintHoursCapacity = 0 //await SprintCapacityManager.GetSprintCapacityById(projectname, teamName, item.IterationSK)
                });
            }

            return iteration;
        }

        public class WorkItemResponse
        {
            [JsonPropertyName("value")]
            public List<Iteration> Value { get; set; }
        }
    }
}

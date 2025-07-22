using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Manager to handle with the iteration entity.
    /// </summary>
    internal static class IterationManager
    {
        /// <summary>
        ///  
        /// </summary>
        /// <param name="projectkey"></param>
        /// <param name="projectname"></param>
        /// <param name="teamName"></param>
        /// <returns></returns>
        internal static async Task<List<Iteration>> GetTfsIterations(string projectkey, string projectname, string teamName)
        {
            return await GetTfsIterations_Internal(projectkey, projectname, teamName, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectkey"></param>
        /// <param name="projectname"></param>
        /// <param name="teamName"></param>
        /// <param name="iterationlevel"></param>
        /// <returns></returns>
        internal static async Task<List<Iteration>> GetTfsIterations(string projectkey, string projectname, string teamName, int iterationlevel)
        {
            return await GetTfsIterations_Internal(projectkey, projectname, teamName, iterationlevel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectkey"></param>
        /// <param name="projectname"></param>
        /// <param name="teamName"></param>
        /// <param name="iterationlevel"></param>
        /// <returns></returns>
        private static async Task<List<Iteration>> GetTfsIterations_Internal(string projectkey, string projectname, string teamName, int iterationlevel)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{string.Empty}:{AppSettings.PersonalAccessToken}")));
                
            string tfsUri = string.Format($"{AppSettings.TfsUri}/{{0}}/_odata/v4.0-preview/Iterations?$select=IterationName,IterationPath,IterationSK,StartDate,EndDate&$filter=IterationLevel{{2}} eq '{{1}}'",projectname,projectkey, iterationlevel);

            var response = await client.GetAsync(tfsUri);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var workItems = JsonSerializer.Deserialize<IterationWorkItemResponse>(responseBody);
            var iteration = new List<Iteration>();

            foreach (var item in workItems.Value)
            {
                iteration.Add(new Iteration
                {
                    PartitionKey = projectkey,
                    RowKey = item.IterationSK.ToString(),
                    IterationName = item.IterationName,
                    IterationPath = item.IterationPath,
                    IterationSK = item.IterationSK,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    SprintHoursCapacity = 0
                });
            }

            return iteration;
        }
    }
    /// <summary>
    /// Allow access to the json response.
    /// </summary>
    internal class IterationWorkItemResponse
    {
        [JsonPropertyName("value")]
        public List<Iteration> Value { get; set; }
    }
}

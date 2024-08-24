using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Manager to handle with the area entity.
    /// </summary>
    internal static class AreaManager
    {
        /// <summary>
        /// Get all the areas for the selected project.
        /// </summary>
        /// <param name="projectname">The project name.</param>
        /// <param name="areapath">The Area or project iteration.</param>
        /// <returns></returns>
        internal static async Task<List<Area>> GetAreas(string projectname, string areapath)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{string.Empty}:{AppSettings.PersonalAccessToken}")));
                
            string tfsUri = string.Format($"{AppSettings.TfsUri}/{{0}}/_odata/v4.0-preview/Areas?$filter=AreaPath eq '{{1}}'&$select=AreaId,AreaSK,AreaPath",projectname,areapath);
            var response = await client.GetAsync(tfsUri);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var workItems = JsonSerializer.Deserialize<WorkItemResponse>(responseBody);
            var areas = new List<Area>();

            foreach (var item in workItems.Value)
            {
                areas.Add(new Area
                {
                    PartitionKey = projectname,
                    RowKey = item.AreaSK.ToString(),
                    AreaPath = item.AreaPath,
                    AreaSK = item.AreaSK    
                });
            }

            return areas;
        }

        /// <summary>
        /// Allow access to the json response.
        /// </summary>
        public class WorkItemResponse
        {
            [JsonPropertyName("value")]
            public List<Area> Value { get; set; }
        }
    }
}

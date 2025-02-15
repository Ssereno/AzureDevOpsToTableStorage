using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Entity that represent an ProjectProfile. 
    /// This entity is not obtained from azuredevops and is filled in manually to support the PBI. 
    /// </summary>
    public class ProjectProfile : AzureTableBase
    {
        [JsonPropertyName("AreaSK")]
        public string AreaSK { get; set; }

        [JsonPropertyName("AreaPath")]
        public string AreaPath { get; set; }

        [JsonPropertyName("IterationName")]
        public string IterationName { get; set; }

        [JsonPropertyName("IterationSK")]
        public string IterationSK { get; set; }

        [JsonPropertyName("Velocity")]
        public int Velocity { get; set; }

        [JsonPropertyName("ProjectEffort")]
        public int ProjectEffort { get; set; }

        [JsonPropertyName("ProjectStartDate")]
        public string ProjectStartDate { get; set; }

        [JsonPropertyName("ProjectEndDate")]
        public string ProjectEndDate { get; set; }

        [JsonPropertyName("ProjectRealEndDate")]
        public string ProjectRealEndDate { get; set; }
    }
}
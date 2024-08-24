using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Entity that represent the sprint capacity.
    /// </summary>
    public class SprintCapacity:AzureTableBase
    {
        [JsonPropertyName("ProjectName")]
        public string ProjectName { get; set; }

        [JsonPropertyName("SprintName")]
        public string SprintName { get; set; }

        [JsonPropertyName("SprintPath")]
        public string SprintPath { get; set; }
        
        [JsonPropertyName("IterationSK")]
        public string IterationSK{ get; set; }

        [JsonPropertyName("SprintHoursCapacity")]
        public double? SprintHoursCapacity{ get; set; }
    }
}
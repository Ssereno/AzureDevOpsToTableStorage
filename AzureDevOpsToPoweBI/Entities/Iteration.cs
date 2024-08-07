using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    public class Iteration:AzureTableBase
    {
        [JsonPropertyName("IterationName")]
        public string IterationName { get; set; }

        [JsonPropertyName("IterationPath")]
        public string IterationPath { get; set; }
        
        [JsonPropertyName("IterationSK")]
        public string IterationSK{ get; set; }

        [JsonPropertyName("StartDate")]
        public string? StartDate{ get; set; }
        
        [JsonPropertyName("EndDate")]
        public string? EndDate{ get; set; }

        public double? SprintHoursCapacity{ get; set; }
    }
}
using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    public class TfsTasks: AzureTableBase
    {
        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("State")]
        public string? State { get; set; }
        
        [JsonPropertyName("WorkItemId")]
        public int? WorkItemId{ get; set; }

        [JsonPropertyName("WorkItemType")]
        public string? WorkItemType{ get; set; }

        [JsonPropertyName("AreaSK")]
        public string? AreaSK{ get; set; }

        [JsonPropertyName("IterationSK")]
        public string? IterationSK{ get; set; }

        [JsonPropertyName("TagNames")]
        public string? TagNames{ get; set; }

        [JsonPropertyName("ParentWorkItemId")]
        public int? ParentWorkItemId{ get; set; }

        [JsonPropertyName("OriginalEstimate")]
        public decimal? OriginalEstimate{ get; set; }

        [JsonPropertyName("CompletedWork")]
        public decimal? CompletedWork{ get; set; }
    }
}
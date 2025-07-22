using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    public class UserStory: AzureTableBase
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("State")]
        public string State { get; set; }
        
        [JsonPropertyName("WorkItemId")]
        public int WorkItemId{ get; set; }

        [JsonPropertyName("WorkItemType")]
        public string WorkItemType{ get; set; }
        
        [JsonPropertyName("StoryPoints")]
        public decimal? StoryPoints{ get; set; }
        
        [JsonPropertyName("LeadTimeDays")]
        public decimal? LeadTimeDays{ get; set; }

        [JsonPropertyName("CycleTimeDays")]
        public decimal? CycleTimeDays{ get; set; }

        [JsonPropertyName("CreatedDate")]
        public string? CreatedDate{ get; set; }

        [JsonPropertyName("ResolvedDate")]
        public string? ResolvedDate{ get; set; }

        [JsonPropertyName("AreaSK")]
        public string AreaSK{ get; set; }

        [JsonPropertyName("IterationSK")]
        public string IterationSK{ get; set; }

        [JsonPropertyName("ActivatedDate")]
        public string? ActivatedDate{ get; set; }

        [JsonPropertyName("ClosedDate")]
        public string? ClosedDate{ get; set; }

        [JsonPropertyName("ParentWorkItemId")]
        public int? ParentWorkItemId{ get; set; }

        [JsonPropertyName("TagNames")]
        public string? TagNames{ get; set; }

        [JsonPropertyName("CompletedDate")]
        public string? CompletedDate { get; set; }
    }
}
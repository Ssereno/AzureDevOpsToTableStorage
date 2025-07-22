using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Entity that represent an bug.
    /// </summary>
    public class Bug: AzureTableBase
    {
        [JsonPropertyName("WorkItemId")]
        public int WorkItemId { get; set; }

        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("State")]
        public string State { get; set; }

        [JsonPropertyName("AreaSK")]
        public string AreaSK { get; set; }

        [JsonPropertyName("IterationSK")]
        public string IterationSK { get; set; }

        [JsonPropertyName("CreatedDate")]
        public string CreatedDate { get; set; }

        [JsonPropertyName("ActivatedDate")]
        public string ActivatedDate { get; set; }

        [JsonPropertyName("ClosedDate")]
        public string ClosedDate { get; set; }

        [JsonPropertyName("ResolvedDate")]
        public string ResolvedDate { get; set; }

        [JsonPropertyName("CompletedDate")]
        public string CompletedDate { get; set; }

        [JsonPropertyName("Severity")]
        public string Severity { get; set; }

        [JsonPropertyName("ParentWorkItemId")]
        public string? ParentWorkItemId { get; set; }
        
        [JsonPropertyName("Custom_BugType")]
        public string? Custom_BugType { get; set; }

        [JsonPropertyName("Custom_TicketID")]
        public string? Custom_TicketID { get; set; }

        [JsonPropertyName("Custom_TicketPriority")]
        public string? Custom_TicketPriority { get; set; }
    }
}
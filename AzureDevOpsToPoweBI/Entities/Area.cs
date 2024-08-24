using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Entity that represent an area.
    /// </summary>
    public class Area: AzureTableBase
    {
        [JsonPropertyName("AreaSK")]
        public string AreaSK { get; set; }

        [JsonPropertyName("AreaPath")]
        public string AreaPath { get; set; }

        [JsonPropertyName("AreaId")]
        public string AreaId { get; set; }
    }
}
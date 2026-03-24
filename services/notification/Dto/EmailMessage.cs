using Newtonsoft.Json;

namespace notification.Dto
{
    public class EmailMessage
    {
        [JsonProperty("Receipient")]
        public string To { get; set; }
        [JsonProperty("Subject")]
        public string Subject { get; set; }
        [JsonProperty("Body")]
        public string Body { get; set; }
        [JsonIgnore]
        public string From { get; set; } = "MS_hQQeMH@test-65qngkdmw38lwr12.mlsender.net";
        [JsonProperty("IsBodyHtml")]
        public bool IsBodyHtml { get; set; }
    }
}

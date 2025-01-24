using System.Text.Json.Serialization;

namespace FacebookAutomation.Models
{
    public class Data
    {
        //[JsonPropertyName("node")]
        //public Node Node { get; set; }
    }

    public class PageInfo
    {
        [JsonPropertyName("end_cursor")]
        public string End_Cursor { get; set; }

        [JsonPropertyName("has_next_page")]
        public bool Has_Next_Page { get; set; }
    }
}

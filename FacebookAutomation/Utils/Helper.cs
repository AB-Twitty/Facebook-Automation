using FacebookAutomation.Models.Facebook;
using Newtonsoft.Json.Linq;

namespace FacebookAutomation.Utils
{
    public class Helper
    {
        public static string GetCursorValue(PageInfo? nextPage)
        {
            return nextPage?.End_Cursor != null && nextPage.Has_Next_Page ? $"\"{nextPage.End_Cursor}\"" : "null";
        }

        public static async Task<JObject> DeserializeResponseToDynamic(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            string[] lines = responseContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string firstJsonObject = lines[0].Trim();
            return JObject.Parse(firstJsonObject);
        }

        public static async Task<string> ExtractJsonFromJavascript(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseContent))
                return "";

            // Regular expression to match the JavaScript code pattern (e.g., for (;;);{...})
            var regex = new System.Text.RegularExpressions.Regex(@"^\s*for\s*\(\s*;;\s*\);\s*(\{.*\})", System.Text.RegularExpressions.RegexOptions.Singleline);
            var match = regex.Match(responseContent);

            // If a match is found, extract the JSON part
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return "";
        }

    }
}

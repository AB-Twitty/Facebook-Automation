﻿using FacebookAutomation.Exceptions;
using FacebookAutomation.Models.Facebook;
using Newtonsoft.Json.Linq;

namespace FacebookAutomation.Utils
{
    public class Helper
    {
        public static string GetCursorValue(Pagination? nextPage)
        {
            if (nextPage == null)
                return "null";

            return !string.IsNullOrEmpty(nextPage.End_Cursor) && nextPage.Has_Next_Page ? $"\"{nextPage.End_Cursor}\"" : "null";
        }

        public static JObject DeserializeJson(string content)
        {
            string[] lines = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string firstJsonObject = lines[0].Trim();
            JObject firstJson = JObject.Parse(firstJsonObject);

            foreach (var line in lines)
            {
                if (line.Contains("\"page_info\":"))
                {
                    int startIndex = line.IndexOf("\"page_info\":");
                    int endIndex = line.IndexOf('}', startIndex) + 1;
                    string pageInfoJson = line.Substring(startIndex, endIndex - startIndex);

                    JObject pageInfoObject = JObject.Parse("{" + pageInfoJson + "}");
                    firstJson.Merge(pageInfoObject, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    break;
                }
            }

            return firstJson;
        }

        public static async Task<JObject> DeserializeResponseToDynamic(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseContent))
                return new JObject();

            // Regular expression to match the JavaScript code pattern (e.g., for (;;);{...})
            var regex = new System.Text.RegularExpressions.Regex(@"^\s*for\s*\(\s*;;\s*\);\s*(\{.*\})", System.Text.RegularExpressions.RegexOptions.Singleline);
            var match = regex.Match(responseContent);

            // If a match is found, extract the JSON part
            if (match.Success)
            {
                var jsonObject = JObject.Parse(match.Groups[1].Value.Trim());
                var fb_dtsg = jsonObject["dtsgToken"]?.ToString();

                if (!string.IsNullOrEmpty(fb_dtsg))
                {
                    FormDataState.Instance.SetFormData("fb_dtsg", fb_dtsg);
                    throw new NewDtsgTokenException();
                }
            }

            return DeserializeJson(responseContent);
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

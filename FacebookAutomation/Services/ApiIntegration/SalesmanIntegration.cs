using FacebookAutomation.Models;
using Newtonsoft.Json;
using System.Text;

namespace FacebookAutomation.Services.ApiIntegration
{
    public class SalesmanIntegration
    {
        private const string BASE_URL = "https://salesmanapi.salesman.tools/api/";
        //private const string BASE_URL = "https://localhost:44335/api/";

        public static async Task UploadToApiAsync(string name, string phone, string mobile, string whatsapp, string taskId)
        {
            var searchResult = new SearchResultModel
            {
                AutomationTaskId = taskId,
                Name = name,
                PhoneNumber = phone,
                Mobile = mobile,
                Whatsapp = whatsapp,
            };

            string apiUrl = $"{BASE_URL}SearchResult/DeductCreditsAndCreateSearchResult";
            var content = new StringContent(JsonConvert.SerializeObject(searchResult), Encoding.UTF8, "application/json");

            using HttpClient client = new();
            try
            {
                var response = await client.PostAsync(apiUrl, content);
                Console.WriteLine($"Response from API: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task MarkTaskAsCompleted(string taskId)
        {
            string completeTaskApiUrl = $"{BASE_URL}AutomationTask/TaskCompleted/{taskId}";

            using HttpClient client = new();
            try
            {
                var response = await client.PutAsync(completeTaskApiUrl, null);
                Console.WriteLine(response.IsSuccessStatusCode
                    ? $"Task {taskId} completed successfully."
                    : $"Failed to complete task {taskId}. Status Code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred while completing task {taskId}: {ex.Message}");
            }
        }
    }
}

using FacebookAutomation.Services.ApiIntegration;

public class Program
{

    public static async Task Main(string[] args)
    {
        var facebookIntegrationService = new FacebookIntegrationService();
        await facebookIntegrationService.GetReatorsForPostByFeedbackId("ZmVlZGJhY2s6MTIyMTY2MDUzMDYwMjczNTM0");
    }

    /*
    public static async Task Main(string[] args)
    {
        var searchParameters = FileManager.GetSearchParameters();

        if (searchParameters == null)
        {
            searchParameters = new SearchParameters();

            Console.WriteLine("Enter search query:");
            searchParameters.SearchQuery = Console.ReadLine();

            Console.WriteLine("Enter number of items:");
            searchParameters.SearchLimit = Console.ReadLine();

            Console.WriteLine("Enter location:");
            searchParameters.Location = Console.ReadLine();

            searchParameters.TaskId = Constants.DEFAULT_TASK_ID;
        }


        await BayutDataFetcher.FetchData(searchParameters);
    }
}

public class BayutDataFetcher
{
    private const int HitsPerPage = 1000;

    public static async Task FetchData(SearchParameters searchParameters)
    {
        string Purpose = searchParameters.SearchQuery;
        List<string> Locations = [searchParameters.Location];

        int totalResults = 0, duplicateCount = 0, nullCount = 0, page = 0;
        bool morePages = true;
        var seenContacts = new HashSet<string>();
        var maxResults = Convert.ToInt32(searchParameters.SearchLimit);

        var initialResponse = await BayutIntegrationService.GetPropertiesAsync(Purpose, Locations, page, HitsPerPage, "sa");
        int nbHits = initialResponse.results.FirstOrDefault()?.nbHits ?? 0;
        Console.WriteLine($"Total results available: {nbHits}");

        while (morePages && totalResults < nbHits && totalResults < maxResults)
        {
            var response = await BayutIntegrationService.GetPropertiesAsync(
                Purpose, Locations, page, HitsPerPage, "om");
            var hits = response.results.FirstOrDefault()?.hits ?? [];

            foreach (var hit in hits)
            {
                if (totalResults >= maxResults)
                {
                    await SalesmanIntegration.MarkTaskAsCompleted(searchParameters.TaskId);
                    break;
                }

                var contactName = hit.contactName ?? "";
                var phone = hit.phoneNumber?.phone ?? "";
                var mobile = hit.phoneNumber?.mobile ?? "";
                var whatsapp = hit.phoneNumber?.whatsapp ?? "";

                string contactKey = $"{contactName}-{phone}-{mobile}-{whatsapp}";

                if (string.IsNullOrWhiteSpace(contactKey.Replace("-", "")))
                {
                    nullCount++;
                    continue;
                }

                if (seenContacts.Contains(contactKey))
                {
                    duplicateCount++;
                    continue;
                }

                await SalesmanIntegration.UploadToApiAsync(contactName, phone, mobile, whatsapp, searchParameters.TaskId);

                seenContacts.Add(contactKey);
                Console.WriteLine($"{contactName}, {phone}, {mobile}, {whatsapp}");
                totalResults++;
            }

            int nbPages = response.results.FirstOrDefault()?.nbPages ?? 0;
            Console.WriteLine($"Page {page + 1} of {nbPages} fetched with {hits.Count} results.");

            page++;
            morePages = page < nbPages;
        }

        await SalesmanIntegration.MarkTaskAsCompleted(searchParameters.TaskId);

        Console.WriteLine($"Total unique results saved: {totalResults}");
        Console.WriteLine($"Total duplicates encountered: {duplicateCount}");
        Console.WriteLine($"Total null entries encountered: {nullCount}");
    }
    */
}
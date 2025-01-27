using FacebookAutomation.Factories;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Services.Facebook;
using Microsoft.Extensions.Hosting;

public class Program
{

    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();


        var postService = new FacebookPostsIntegrationService();

        await postService.ReactOnPost(new FacebookAutomation.Models.PostInfoModel
        {
            StoryId = "UzpfSUZTOjE6LTQ3NDMyMjcxNzE4NzAxNDAyNjplSndUNjVnMmVmR0VXYmQzZkdYVWxCRzcxM3pyMTVYZXVTeWlmR0x2cm41YXZXNi9ydGl6UlN0LzhvaHQycjcxSjQ4MG85c0tlVFZOR1pGM3pHb01wbUFKTXdZeGhqU28wdFZNYmxmRW1YNHhmVHU5c0FOa2lEU2p4NVFHSnFiRUlyaXgwZ3dNRE9JT3IvT2ZNREF3TUtnS1NUTzVTY0drN2drd21hMi90UGJBcTZZZjd5WXlwaktJc3VoSU16YnlaUmJIRjZlV3hHZm14YWVscHFZd1NqTmdod3c4WG1JTU1VSkNEQXhKcGd3TUFCbjZUUUk9",
            FeedbackId = "ZmVlZGJhY2s6MTE4OTkwNjMyMjUwNTk2Nw==",
            PostId = "1189906202505979"
        }, Reactions.LOVE);


        //await FacebookDataFetcher.FetchData("rdr 2", "2000");

        await FacebookLogoutAutomation.Logout();
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
    }*/

    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Register services

                });


    public class FacebookDataFetcher
    {
        public static async Task FetchData(string search, string searchLimit)
        {
            int totalResults = 0, duplicateCount = 0, nullCount = 0, page = 0;
            var seenContacts = new HashSet<string>();
            var maxResults = Convert.ToInt32(searchLimit);

            foreach (var type in Enum.GetValues(typeof(FacebookIntegrationServiceType)))
            {
                var facebookIntegrationService = FacebookIntegrationFactory.GetFacebookIntegrationService((FacebookIntegrationServiceType)type);
                BaseResponse<BaseResponseModel>? searchResponse = null;

                bool maxReached = false;
                Pagination? nextPage = null;

                do
                {
                    searchResponse = await facebookIntegrationService.SendSearchRequestAsync(search, nextPage);
                    if (searchResponse == null || searchResponse.Models == null || !searchResponse.Models.Any())
                        break;

                    nextPage = searchResponse.Pagination;
                    foreach (var model in searchResponse.Models)
                    {
                        if (maxReached)
                            break;

                        // Fetch users associated with the current model
                        BaseResponse<FacebookUser> usersResponse;
                        Pagination? usersNextPage = null;

                        do
                        {
                            usersResponse = await facebookIntegrationService.GetFacebookUsersFor(model, usersNextPage);
                            if (usersResponse == null || usersResponse.Models == null || !usersResponse.Models.Any())
                                break;

                            usersNextPage = usersResponse.Pagination;

                            foreach (var user in usersResponse.Models)
                            {
                                string contactKey = $"{user.Id}-{user.Name}";

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

                                // Process valid contact
                                seenContacts.Add(contactKey);
                                Console.WriteLine(contactKey);
                                totalResults++;

                                if (totalResults >= maxResults)
                                {
                                    // We have reached the max result limit, no need to continue.
                                    return;
                                }
                            }
                        }
                        while (usersResponse.Pagination.Has_Next_Page);

                        if (totalResults >= maxResults)
                        {
                            maxReached = true;
                            break;
                        }
                    }

                }
                while (!maxReached && searchResponse.Pagination.Has_Next_Page);
            }
        }
    }

    /*
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
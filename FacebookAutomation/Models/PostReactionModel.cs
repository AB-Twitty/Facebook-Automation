using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Models
{
    public class PostReactionsResult
    {
        public PageInfo PageInfo { get; set; }
        public IList<FacebookUser> Reactors { get; set; }
    }

    public class PostReactionsMapper
    {
        public PostReactionsResult MapToPostInfoResult(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new PostReactionsResult
            {
                Reactors = new List<FacebookUser>()
            };

            // Map the results (edges) into PostModels
            if (expandoObject?.data?.node?.reactors?.edges != null)
            {
                foreach (var edge in expandoObject.data.node.reactors.edges)
                {
                    var user = new FacebookUser
                    {
                        Id = edge.node?.id,
                        Name = edge.node?.name
                    };

                    result.Reactors.Add(user);
                }
            }

            if (expandoObject?.data?.node?.reactors?.page_info != null)
            {
                result.PageInfo = new PageInfo
                {
                    End_Cursor = expandoObject.data.node.reactors.page_info.end_cursor,
                    Has_Next_Page = expandoObject.data.node.reactors.page_info.has_next_page
                };
            }

            return result;
        }
    }
}

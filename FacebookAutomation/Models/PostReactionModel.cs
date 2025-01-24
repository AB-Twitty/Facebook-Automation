namespace FacebookAutomation.Models
{
    public class PostReactionsResult
    {
        public PageInfo PageInfo { get; set; }
        public IList<ReactorInfo> Reactors { get; set; }
    }
    public class ReactorInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class PostReactionsMapper
    {
        public PostReactionsResult MapToPostInfoResult(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new PostReactionsResult
            {
                Reactors = new List<ReactorInfo>()
            };

            // Map the results (edges) into PostModels
            if (expandoObject?.data?.node?.reactors?.edges != null)
            {
                foreach (var edge in expandoObject.data.node.reactors.edges)
                {
                    var postInfo = new ReactorInfo
                    {
                        Id = edge.node?.id,
                        Name = edge.node?.name
                    };

                    result.Reactors.Add(postInfo);
                }
            }

            if (expandoObject?.data?.node?.results?.page_info != null)
            {
                result.PageInfo = new PageInfo
                {
                    End_Cursor = expandoObject.data.node.results.page_info.end_cursor,
                    Has_Next_Page = expandoObject.data.node.results.page_info.has_next_page
                };
            }

            return result;
        }
    }
}

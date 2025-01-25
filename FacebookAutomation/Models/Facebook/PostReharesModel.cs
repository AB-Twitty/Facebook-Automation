namespace FacebookAutomation.Models.Facebook
{
    public class PostResharesModel
    {
        public Pagination Pagination { get; set; }
        public IList<FacebookUser> Reshares_Users { get; set; }
    }

    public class PostResharesMapper
    {
        public PostResharesModel MapToPostInfoResult(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new PostResharesModel
            {
                Reshares_Users = new List<FacebookUser>()
            };

            // Map the results (edges) into PostModels
            if (expandoObject?.data?.node?.reshares?.edges != null)
            {
                foreach (var edge in expandoObject.data.node.reshares.edges)
                {
                    var user = new FacebookUser
                    {
                        Id = edge.node?.comet_sections?.context_layout?.story?.comet_sections?.title?.story?.actors[0]?.id,
                        Name = edge.node?.comet_sections?.context_layout?.story?.comet_sections?.title?.story?.actors[0]?.name
                    };

                    result.Reshares_Users.Add(user);
                }
            }

            if (expandoObject?.data?.node?.reshares?.page_info != null)
            {
                result.Pagination = new Pagination
                {
                    End_Cursor = expandoObject.data.node.reshares.page_info.end_cursor,
                    Has_Next_Page = expandoObject.data.node.reshares.page_info.has_next_page
                };
            }

            return result;
        }
    }
}

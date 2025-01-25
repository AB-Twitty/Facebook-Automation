namespace FacebookAutomation.Models.Facebook
{
    public class PostCommentsModel
    {
        public Pagination Pagination { get; set; }
        public IList<FacebookUser> CommentsUsers { get; set; }
    }

    public class PostCommentsMapper
    {
        public PostCommentsModel MapToPostInfoResult(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new PostCommentsModel
            {
                CommentsUsers = new List<FacebookUser>()
            };

            // Map the results (edges) into PostModels
            if (expandoObject?.data?.node?.comment_rendering_instance_for_feed_location?.comments?.edges != null)
            {
                foreach (var edge in expandoObject.data.node.comment_rendering_instance_for_feed_location.comments.edges)
                {
                    var user = new FacebookUser
                    {
                        Id = edge.node?.author?.id,
                        Name = edge.node?.author?.name
                    };

                    result.CommentsUsers.Add(user);
                }
            }

            if (expandoObject?.data?.node?.comment_rendering_instance_for_feed_location?.comments?.page_info != null)
            {
                result.Pagination = new Pagination
                {
                    End_Cursor = expandoObject.data.node.comment_rendering_instance_for_feed_location.comments.page_info.end_cursor,
                    Has_Next_Page = expandoObject.data.node.comment_rendering_instance_for_feed_location.comments.page_info.has_next_page
                };
            }

            return result;
        }
    }
}

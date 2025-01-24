namespace FacebookAutomation.Models
{
    public class PostInfoResult
    {
        public PageInfo PageInfo { get; set; }
        public IList<PostInfoModel> PostModels { get; set; }
    }
    public class PostInfoModel
    {
        public string StoryId { get; set; }
        public string PostId { get; set; }
        public string FeedbackId { get; set; }
    }

    public class PostMapper
    {
        public PostInfoResult MapToPostInfoResult(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new PostInfoResult
            {
                PostModels = new List<PostInfoModel>()
            };

            // Map the results (edges) into PostModels
            if (expandoObject?.data?.serpResponse?.results?.edges != null)
            {
                foreach (var edge in expandoObject.data.serpResponse.results.edges)
                {
                    var postInfo = new PostInfoModel
                    {
                        StoryId = edge.rendering_strategy?.view_model?.click_model?.story?.id,
                        PostId = edge.rendering_strategy?.view_model?.click_model?.story?.post_id,
                        FeedbackId = edge.rendering_strategy?.view_model?.click_model?.story?.feedback?.id
                    };

                    result.PostModels.Add(postInfo);
                }
            }

            if (expandoObject?.data?.serpResponse?.results?.page_info != null)
            {
                result.PageInfo = new PageInfo
                {
                    End_Cursor = expandoObject.data.serpResponse.results.page_info.end_cursor,
                    Has_Next_Page = expandoObject.data.serpResponse.results.page_info.has_next_page
                };
            }

            return result;
        }
    }

}

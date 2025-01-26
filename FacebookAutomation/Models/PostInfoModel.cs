using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Models
{
    public class PostInfoModel : BaseResponseModel
    {
        public string? StoryId { get; set; }
        public string? PostId { get; set; }
        public string? FeedbackId { get; set; }
    }

    public class PostMapper
    {
        public BaseResponse<PostInfoModel> MapToPostInfoResult(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new BaseResponse<PostInfoModel>
            {
                Models = new List<PostInfoModel>()
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

                    result.Models.Add(postInfo);
                }
            }

            if (expandoObject?.data?.serpResponse?.results?.page_info != null)
            {
                result.Pagination = new Pagination
                {
                    End_Cursor = expandoObject.data.serpResponse.results.page_info.end_cursor,
                    Has_Next_Page = expandoObject.data.serpResponse.results.page_info.has_next_page
                };
            }

            return result;
        }
    }


    public class PostFromPageMapper
    {
        public BaseResponse<PostInfoModel> MapToPostInfoResult(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new BaseResponse<PostInfoModel>
            {
                Models = new List<PostInfoModel>()
            };

            // Map the results (edges) into PostModels
            if (expandoObject?.data?.node?.timeline_list_feed_units?.edges != null)
            {
                foreach (var edge in expandoObject.data.node.timeline_list_feed_units.edges)
                {
                    var postInfo = new PostInfoModel
                    {
                        StoryId = edge.node?.id,
                        PostId = edge.node?.post_id,
                        FeedbackId = edge.node?.feedback?.id
                    };

                    result.Models.Add(postInfo);
                }
            }

            if (expandoObject?.data?.node?.timeline_list_feed_units?.edges[0]?.cursor != null)
            {
                result.Pagination = new Pagination
                {
                    End_Cursor = expandoObject?.data?.node?.timeline_list_feed_units?.edges[0]?.cursor,
                    Has_Next_Page = true
                };

                result.Pagination.Has_Next_Page = !string.IsNullOrEmpty(result.Pagination.End_Cursor);
            }

            return result;
        }
    }

}

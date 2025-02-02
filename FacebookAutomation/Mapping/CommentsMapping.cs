using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Mapping
{
    public class PostCommentsMapper : FacebookResponseMapper<FacebookUser>
    {
        protected override BaseResponse<FacebookUser> MapToModel(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new BaseResponse<FacebookUser>
            {
                Models = new List<FacebookUser>()
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

                    result.Models.Add(user);
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


    public class CommentsMapper : FacebookResponseMapper<CommentModel>
    {
        protected override BaseResponse<CommentModel> MapToModel(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new BaseResponse<CommentModel>
            {
                Models = new List<CommentModel>()
            };

            // Map the results (edges) into PostModels
            if (expandoObject?.data?.node?.comment_rendering_instance_for_feed_location?.comments?.edges != null)
            {
                foreach (var edge in expandoObject.data.node.comment_rendering_instance_for_feed_location.comments.edges)
                {
                    var comment = new CommentModel
                    {
                        Id = edge.node?.id,
                        FeedbackId = edge.node?.feedback?.id,
                        CommentId = edge.node?.legacy_fbid,
                        Text = edge.node?.body?.text,
                        CommenterId = edge.node?.author?.id,
                        CommenterName = edge.node?.author?.name,
                        RepliesCount = edge.node?.feedback?.replies_fields?.total_count,
                        CommentTime = edge.node?.created_time != null
                            ? DateTimeOffset.FromUnixTimeSeconds(edge.node?.created_time.Value).DateTime
                            : null,
                        ReactionsCount = edge.node?.comment_action_links?[1]?.comment?.feedback?.reactors?.count,
                        Url = edge.node?.feedback?.url
                    };

                    result.Models.Add(comment);
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

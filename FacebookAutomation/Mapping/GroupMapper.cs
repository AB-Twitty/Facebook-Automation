using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Mapping
{
    public class GroupMapper : FacebookResponseMapper<GroupInfoModel>
    {
        protected override BaseResponse<GroupInfoModel> MapToModel(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new BaseResponse<GroupInfoModel>
            {
                Models = new List<GroupInfoModel>()
            };

            if (expandoObject?.data?.serpResponse?.results?.edges != null)
            {
                foreach (var edge in expandoObject.data.serpResponse.results.edges)
                {
                    var postInfo = new GroupInfoModel
                    {
                        Id = edge.rendering_strategy?.view_model?.profile?.id,
                        Name = edge.rendering_strategy?.view_model?.profile?.name,
                        Url = edge.rendering_strategy?.view_model?.profile?.url,
                        JoinState = edge.rendering_strategy?.view_model?.ctas?.primary?[0]?.profile?.viewer_join_state,
                        PublicityState = edge.rendering_strategy?.view_model?.ctas?.primary?[0]?.profile?.viewer_forum_join_state
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


    public class GroupMembersMapper : FacebookResponseMapper<FacebookUser>
    {
        protected override BaseResponse<FacebookUser> MapToModel(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new BaseResponse<FacebookUser>
            {
                Models = new List<FacebookUser>()
            };

            if (expandoObject?.data?.node?.new_members?.edges != null)
            {
                foreach (var edge in expandoObject.data.node.new_members.edges)
                {
                    var postInfo = new FacebookUser
                    {
                        Id = edge.node?.id,
                        Name = edge.node?.name,
                        Url = edge.node?.url,
                        FriendshipStatus = edge.node?.user_type_renderer?.user?.friendship_status
                    };

                    result.Models.Add(postInfo);
                }
            }

            if (expandoObject?.data?.node?.new_members?.page_info != null)
            {
                result.Pagination = new Pagination
                {
                    End_Cursor = expandoObject.data.node.new_members.page_info.end_cursor,
                    Has_Next_Page = expandoObject.data.node.new_members.page_info.has_next_page
                };
            }

            return result;
        }
    }
}

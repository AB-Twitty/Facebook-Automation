using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Mapping
{
    public class PostReactionsMapper : FacebookResponseMapper<FacebookUser>
    {
        protected override BaseResponse<FacebookUser> MapToModel(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new BaseResponse<FacebookUser>
            {
                Models = new List<FacebookUser>()
            };

            // Map the results (edges) into PostModels
            if (expandoObject?.data?.node?.reactors?.edges != null)
            {
                foreach (var edge in expandoObject.data.node.reactors.edges)
                {
                    var user = new FacebookUser
                    {
                        Id = edge.node?.id,
                        Name = edge.node?.name,
                        FriendshipStatus = edge.node?.friendship_status,
                        FollowStatus = edge.node?.subscribe_status
                    };

                    result.Models.Add(user);
                }
            }

            if (expandoObject?.data?.node?.reactors?.page_info != null)
            {
                result.Pagination = new Pagination
                {
                    End_Cursor = expandoObject.data.node.reactors.page_info.end_cursor,
                    Has_Next_Page = expandoObject.data.node.reactors.page_info.has_next_page
                };
            }

            return result;
        }
    }
}

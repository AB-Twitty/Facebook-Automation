using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Mapping
{
    public class PostResharesMapper : FacebookResponseMapper<FacebookUser>
    {
        protected override BaseResponse<FacebookUser> MapToModel(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new BaseResponse<FacebookUser>
            {
                Models = new List<FacebookUser>()
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

                    result.Models.Add(user);
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

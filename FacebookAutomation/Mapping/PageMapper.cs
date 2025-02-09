﻿using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Mapping
{
    public class PageMapper : FacebookResponseMapper<PageInfoModel>
    {
        protected override BaseResponse<PageInfoModel> MapToModel(dynamic expandoObject)
        {
            // Initialize the result object
            var result = new BaseResponse<PageInfoModel>
            {
                Models = new List<PageInfoModel>()
            };

            if (expandoObject?.data?.serpResponse?.results?.edges != null)
            {
                foreach (var edge in expandoObject.data.serpResponse.results.edges)
                {
                    var postInfo = new PageInfoModel
                    {
                        Id = edge.rendering_strategy?.view_model?.profile?.id,
                        Name = edge.rendering_strategy?.view_model?.profile?.name
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
}

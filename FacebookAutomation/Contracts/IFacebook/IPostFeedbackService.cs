﻿using FacebookAutomation.Models;
using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Contracts.IFacebook
{
    public interface IPostFeedbackService
    {
        public Task<BaseResponse<FacebookUser>> GetUsersWithFeedbackOnPost(PostInfoModel postInfo, Pagination? pageInfo = null);
        public void UpdateHttpClient(HttpClient httpClient);
    }
}

namespace FacebookAutomation.Contracts.IFacebook
{
    public interface IFacebookUserActionsService
    {
        public Task<bool> FollowUser(string userId);
        public Task<bool> PokeUser(string userId);
        public Task SendFriendRequest(string userId);
        public Task SendBulkFriendRequests(List<string> userIds);
    }
}

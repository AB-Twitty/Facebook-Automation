using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Contracts.IFacebook.IFeedback_Services
{
    public interface IReactionsService
    {
        public Task<bool> ReactTo(string feedbackId, ReactionsEnum reactEnum);
    }
}

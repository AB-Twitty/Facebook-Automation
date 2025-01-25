using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Services.Facebook;

namespace FacebookAutomation.Factories
{
    public static class FacebookIntegrationFactory
    {
        public static IFacebookIntegrationService GetFacebookIntegrationService(FacebookIntegrationServiceType type)
        {
            switch (type)
            {
                case FacebookIntegrationServiceType.Posts:
                    return new FacebookPostsIntegrationService();
                default:
                    return null;
            }
        }
    }

    public enum FacebookIntegrationServiceType
    {
        Posts,
        Pages,
        Groups
    }
}

using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Services.Facebook;

namespace FacebookAutomation.Factories
{
    public static class FacebookIntegrationFactory
    {
        public static IFacebookIntegrationService GetFacebookIntegrationService(FacebookIntegrationServiceType type)
        {
            return type switch
            {
                FacebookIntegrationServiceType.Posts => new FacebookPostsIntegrationService(),
                FacebookIntegrationServiceType.Pages => new FacebookPagesIntegrationService(),
                _ => throw new Exception("Invalid FacebookIntegrationServiceType")
            };
        }
    }

    public enum FacebookIntegrationServiceType
    {
        Pages,
        Posts,

        Groups
    }
}

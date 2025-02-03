namespace FacebookAutomation.Models.Facebook
{
    public class GroupInfoModel : BaseResponseModel
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public string? Url { get; set; }
        public string? JoinState { get; set; }
        public string? PublicityState { get; set; }
        public bool IsMember
        {
            get
            {
                if (string.IsNullOrEmpty(JoinState))
                    return false;

                return JoinState == "MEMBER";
            }
        }
        public bool HasRequestedToJoin
        {
            get
            {
                if (string.IsNullOrEmpty(JoinState))
                    return false;
                return JoinState == "REQUESTED" || JoinState == "MEMBER";
            }
        }
        public bool? IsPublicToJoin
        {
            get
            {
                if (string.IsNullOrEmpty(PublicityState))
                    return null;

                if (PublicityState == "CAN_JOIN") return true;
                else if (PublicityState == "CANNOT_JOIN") return false;
                return null;
            }
        }
    }
}

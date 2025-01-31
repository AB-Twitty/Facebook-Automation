namespace FacebookAutomation.Models.Facebook
{
    public static class Reactions
    {
        public const string LIKE = "1635855486666999";
        public const string LOVE = "1678524932434102";
        public const string CARE = "613557422527858";
        public const string SAD = "908563459236466";
        public const string HAHA = "115940658764963";
        public const string WOW = "478547315650144";
        public const string ANGRY = "444813342392137";

        public static string GetReactionId(ReactionsEnum? reaction)
        {
            return reaction switch
            {
                ReactionsEnum.LIKE => LIKE,
                ReactionsEnum.LOVE => LOVE,
                ReactionsEnum.CARE => CARE,
                ReactionsEnum.SAD => SAD,
                ReactionsEnum.HAHA => HAHA,
                ReactionsEnum.WOW => WOW,
                ReactionsEnum.ANGRY => ANGRY,
                _ => ""
            };
        }
    }

    public enum ReactionsEnum
    {
        LIKE,
        LOVE,
        CARE,
        SAD,
        HAHA,
        WOW,
        ANGRY
    }
}
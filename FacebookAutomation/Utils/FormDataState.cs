namespace FacebookAutomation.Utils
{
    public class FormDataState
    {
        private static readonly Lazy<FormDataState> _instance = new Lazy<FormDataState>(() => new FormDataState());

        public static FormDataState Instance => _instance.Value;

        public string Fb_Dtsg { get; private set; }
        public const string Lsd = "AVo2Q6Qv";
        public const string Jazoest = "22058";
        public const bool ServerTimestamps = false;

        public Dictionary<string, string> GetBaseFormData()
        {
            return new Dictionary<string, string>
            {
                { "fb_dtsg", Fb_Dtsg },
                { "server_timestamps", ServerTimestamps.ToString() },
                { "lsd", Lsd },
                { "jazoest", Jazoest },
                { "__a", "1" }
            };
        }

        public void SetDtsgToken(string dtsgToken)
        {
            Fb_Dtsg = dtsgToken;
        }
    }
}

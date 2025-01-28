namespace FacebookAutomation.Utils
{
    public class FormDataState
    {
        private static readonly Lazy<FormDataState> _instance = new Lazy<FormDataState>(() => new FormDataState());

        public static FormDataState Instance => _instance.Value;

        private readonly Dictionary<string, string> _formData;

        private IList<string> KeysToExclude =>
        [
            "fb_api_req_friendly_name",
            "variables",
            "doc_id"
        ];

        private FormDataState()
        {
            _formData = new Dictionary<string, string>();
        }

        public void SetFormData(string key, string value)
        {
            if (KeysToExclude.Contains(key))
            {
                return;
            }

            if (_formData.ContainsKey(key))
            {
                _formData[key] = value;
            }
            else
            {
                _formData.Add(key, value);
            }
        }

        public string GetFormData(string key)
        {
            return _formData.ContainsKey(key) ? _formData[key] : "";
        }

        public Dictionary<string, string> GetAllFormData()
        {
            return _formData;
        }

        public string GetUserId()
        {
            return GetFormData("__user");
        }
    }
}
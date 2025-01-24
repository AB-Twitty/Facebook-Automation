using FacebookAutomation.Params;

namespace FacebookAutomation.Services.FileService
{
    public static class FileManager
    {
        private static readonly string iniTaskPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "task.ini");

        public static SearchParameters GetSearchParameters()
        {
            if (!File.Exists(iniTaskPath))
                return null;

            var logLines = File.ReadAllLines(iniTaskPath);

            return new SearchParameters
            {
                TaskId = logLines[0],
                SearchQuery = logLines[1],
                SearchLimit = logLines[2],
                Location = logLines[3],
            };
        }
    }
}

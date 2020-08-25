namespace TestConnectionTool
{
    public static class Logger
    {
        public static void ClearLog() =>
            MainViewModel.Instance.TextLog = string.Empty;

        public static void Write(string content) =>
            MainViewModel.Instance.TextLog += content;

        public static void WriteLine(string content = "") =>
            MainViewModel.Instance.TextLog += $"{content}\n";
    }
}

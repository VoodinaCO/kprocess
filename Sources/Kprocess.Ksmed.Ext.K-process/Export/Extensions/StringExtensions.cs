namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    public static class StringExtensions
    {
        static char[] forbiddenCharactersOnWindows = { '\\', '/', ':', '?', '"', '<', '>', '|' };
        static char replacementCharacter = ' ';

        public static string FormatForWindows(this string input)
        {
            string output = input;
            forbiddenCharactersOnWindows.ForEach(c => output = output.Replace(c, replacementCharacter));
            return output;
        }
    }
}

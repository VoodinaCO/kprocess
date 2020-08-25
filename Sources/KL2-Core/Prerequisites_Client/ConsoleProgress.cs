using System;

namespace Prerequisites_Client
{
    public static class ConsoleProgress
    {
        public static void Write(string format, params object[] args)
        {
            var left = Console.CursorLeft;
            var top = Console.CursorTop;
            var width = Console.WindowWidth - 1;
            Console.Write("".PadRight(width - left));
            Console.SetCursorPosition(left, top);
            Console.Write(string.Format(format, args).PadRight(width - left).Substring(0, width - left));
            Console.SetCursorPosition(left, top);
        }

        public static void Finish(string format, params object[] args)
        {
            var left = Console.CursorLeft;
            var top = Console.CursorTop;
            var width = Console.WindowWidth - 1;
            Console.Write("".PadRight(width - left));
            Console.SetCursorPosition(left, top);
            Console.WriteLine(string.Format(format, args).PadRight(width - left).Substring(0, width - left));
        }
    }
}

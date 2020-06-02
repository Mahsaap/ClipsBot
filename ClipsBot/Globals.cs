using System;

namespace ClipsBot
{
    public class Version
    {
        public static string Major { get { return "1"; } }
        public static string Minor { get { return "0"; } }
    }

    public class DiscordIDs
    {
        public static ulong Mahsaap { get { return 88798728948809728; } }
        public static ulong Nizcik { get { return 290501197255671809; } }
    }

    static class Globals
    {
        public static string CurrentTime { get { return DateTime.Now.ToString("HH:mm:ss"); } }
    }
}
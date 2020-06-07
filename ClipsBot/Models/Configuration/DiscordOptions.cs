namespace ClipsBot.Models.Configuration
{
    public class DiscordOptions
    {
        public ulong[] AdminIds { get; set; }
        public Channels ChannelList { get; set; }
        public string Token { get; set; }
    }

    public class Channels
    {
        public ulong CheckChannel { get; set; }
        public ulong ToChannel { get; set; }
    }
}

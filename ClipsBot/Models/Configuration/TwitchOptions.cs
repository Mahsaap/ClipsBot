namespace ClipsBot.Models.Configuration
{
    public class TwitchOptions
    {
        public Credentials Credentials { get; set; }
    }

    public class Credentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}

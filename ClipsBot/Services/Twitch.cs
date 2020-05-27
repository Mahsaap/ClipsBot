using ClipsBot.Ignore;
using TwitchLib.Api;


namespace ClipsBot.Services
{
    internal class Twitch
    {
        public TwitchAPI API { get; }

        public Twitch()
        {
            API = new TwitchAPI();

            API.Settings.ClientId = TwitchCreds.ClientID;
            API.Settings.Secret = TwitchCreds.Secret;
        }
    }
}

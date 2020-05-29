using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.HttpCallHandlers;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Core.RateLimiter;
using TwitchLib.Api.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTwitchLibApi(this IServiceCollection services, string clientId, string clientSecret, int ratelimit)
        {
            services.TryAddSingleton<IApiSettings>(x => new ApiSettings
            {
                ClientId = clientId,
                Secret = clientSecret,
                SkipAutoServerTokenGeneration = false
            });
            services.TryAddSingleton<IRateLimiter>(x => TimeLimiter.GetFromMaxCountByInterval(ratelimit, TimeSpan.FromSeconds(60)));
            services.TryAddSingleton<IHttpCallHandler, TwitchHttpClient>();

            services.TryAddSingleton<ITwitchAPI, TwitchAPI>();

            return services;
        }
    }
}
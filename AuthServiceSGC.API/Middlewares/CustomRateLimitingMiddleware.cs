using AspNetCoreRateLimit;
using AuthServiceSGC.API.Models;
using AuthServiceSGC.Application.Services;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AuthServiceSGC.API.Middlewares
{
    public class CustomRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDatabase _redisDb;
        private readonly IRateLimitConfiguration _rateLimitConfig;
        private readonly IOptions<CustomRateLimitingModel> _customRateLimitingModel;
        private readonly RLMiddlewareFlagService _rLMiddlewareFlagService;
        private readonly IpRateLimitProcessor _ipRateLimitProcessor;

        // Define the max time rate limiting middleware can run
        public const int timeoutMilliseconds = 100;

        public CustomRateLimitingMiddleware(RequestDelegate next, IDatabase redisDb, IRateLimitConfiguration rateLimitConfig, IOptions<CustomRateLimitingModel> customRateLimitingModel, RLMiddlewareFlagService rLMiddlewareFlagService, IpRateLimitProcessor ipRateLimitProcessor)
        {
            _next = next;
            _redisDb = redisDb;
            _rateLimitConfig = rateLimitConfig;
            _customRateLimitingModel = customRateLimitingModel;
            _rLMiddlewareFlagService = rLMiddlewareFlagService;
            _ipRateLimitProcessor = ipRateLimitProcessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (_rLMiddlewareFlagService.FlagValue)
                {

                    //const int timeoutMilliseconds = 100;

                    // Set up a cancellation token source with a timeout
                    using (var cts = new CancellationTokenSource(timeoutMilliseconds))
                    {
                        // Create a task to handle the rate-limiting logic
                        var rateLimitingTask = Task.Run(async () => {
                            // extract endpoint
                            string endpoint = context.Request.Path.ToString();

                            // Get rate limiting configuration for the current endpoint
                            var rateLimitConfig = GetRateLimitConfig(endpoint);
                            if (rateLimitConfig == null)
                            {
                                //If no rate limit configuration is found for this endpoint, allow the request
                                await _next(context);
                                return;
                            }

                            //Comented as this IsIpRequestAllowedOnlyIP method calling inbuilt method is having a null object exception
                            // for only IP type use existing library which uses memory-caching approach 
                            /*
                            if (rateLimitConfig.Type == 0)
                            {
                                // IP-based rate limiting (Type = 0)
                                bool AllowRequest = await IsIpRequestAllowedOnlyIP(context, endpoint, rateLimitConfig);
                                if (!AllowRequest)
                                {
                                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                                    await context.Response.WriteAsync("IP Rate limit exceeded. Please try again later.");
                                    return;
                                }
                            }
                            */

                            // Extract the real IP address using the existing RateLimitConfiguration
                            var ipAddress = ResolveRealIpAddress(context);
                            //Thread.Sleep(5000);
                            string userId = null;

                            // Determine if userId is needed based on rate limit type
                            if (rateLimitConfig.Type == 1 || rateLimitConfig.Type == 2)
                            {
                                userId = await GetUserIdAsync(context.Request);
                                if (string.IsNullOrEmpty(userId))
                                {
                                    // If userId is required but not available, we can choose to:
                                    // a) Deny the request
                                    // b) Treat it as unauthenticated and apply IP-based limits
                                    // Here, we are denying the request
                                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                    await context.Response.WriteAsync("Unauthorized: User ID is required for this endpoint.");
                                    return;
                                }
                            }
                            // Generate a cache key based on the type of rate limiting
                            string cacheKey = GenerateCacheKey(rateLimitConfig.Type, userId, ipAddress, endpoint);

                            // can add a timer , if response is not reverted in 15ms , exist with success code / await _next(context)
                            bool isRequestAllowed = await IsRequestAllowedAsync(cacheKey, rateLimitConfig.Limit, rateLimitConfig.PeriodInSeconds);
                            if (!isRequestAllowed)
                            {
                                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                                return;
                            }
                            /*
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            await context.Response.WriteAsync("ratelimiting is working");
                            return;
                            */
                            await _next(context);

                        }, cts.Token);

                        // Wait for either the rate-limiting task to complete or the timeout to occur
                        var completedTask = await Task.WhenAny(rateLimitingTask, Task.Delay(timeoutMilliseconds));

                        // If the task didn't complete in time, proceed with the next middleware
                        if (completedTask != rateLimitingTask)
                        {
                            await _next(context);
                        }
                    }
                }
                else
                {
                    // flag not true , log the flag check if required
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                // log exception & Also decide - wether to contiue with code (allow hit) / return 500 status code
                await _next(context);
            }
        }

        private EndpointRateLimitConfig GetRateLimitConfig(string endpoint)
        {
            try
            {
                // Try to get the endpoint configuration or group name from the configuration
                if (_customRateLimitingModel.Value.Endpoints.TryGetValue(endpoint, out EndpointOrGroupReference endpointOrGroup))
                {
                    if (!string.IsNullOrEmpty(endpointOrGroup.Group))
                    {
                        _customRateLimitingModel.Value.Groups.TryGetValue(endpointOrGroup.Group, out EndpointRateLimitConfig groupConfig);
                        return groupConfig;
                    }
                    // Don't add endpoint without adding it's config, else this whole block crashes, but catch block above allows the hit
                    else
                    {
                        return new EndpointRateLimitConfig
                        {
                            Limit = endpointOrGroup.Limit.Value,
                            PeriodInSeconds = endpointOrGroup.PeriodInSeconds.Value,
                            Type = endpointOrGroup.Type.Value
                        };
                    }
                }
                return null;
            }
            catch
            {
                // log exception
                return null;
            }
        }

        private string ResolveRealIpAddress(HttpContext context)
        {
            // Utilize the existing RateLimitConfiguration to resolve the real IP address
            foreach (var resolver in _rateLimitConfig.IpResolvers)
            {
                var ip = resolver.ResolveIp(context);
                if (!string.IsNullOrEmpty(ip))
                {
                    return ip;
                }
            }
            return context.Connection.RemoteIpAddress?.ToString();
        }
        private async Task<string> GetUserIdAsync(HttpRequest request)
        {

            //return CommonMethod.GetIMID(request);
            return null;
        }

        private string GenerateCacheKey(int type, string userId, string ipAddress, string endpoint)
        {
            return type switch
            {
                // 0 => $"{ipAddress}:{endpoint}",      // IP-based
                1 => $"{userId}:{endpoint}",         // UID-based
                2 => $"{userId}:{ipAddress}:{endpoint}", // Both IP & UID-based
                _ => throw new InvalidOperationException("Invalid rate limit type."),
            };
        }

        private async Task<bool> IsRequestAllowedAsync(string cacheKey, int limit, int periodInSeconds)
        {
            var currentCount = await _redisDb.StringGetAsync(cacheKey);

            if (int.TryParse(currentCount, out int count))
            {
                if (count >= limit)
                {
                    return false;
                }
                await _redisDb.StringIncrementAsync(cacheKey);
            }
            else
            {
                await _redisDb.StringSetAsync(cacheKey, "1", TimeSpan.FromSeconds(periodInSeconds));
            }

            return true;
        }


        private async Task<bool> IsIpRequestAllowedOnlyIP(HttpContext context, string Endpoint, EndpointRateLimitConfig rateLimitConfig)
        {
            var identity = new ClientRequestIdentity
            {
                //ClientId = " ",
                ClientIp = ResolveRealIpAddress(context),
                Path = context.Request.Path.ToString(),
                HttpVerb = context.Request.Method
            };

            var quotaExceededResponse = new QuotaExceededResponse();

            quotaExceededResponse.ContentType = "String";
            quotaExceededResponse.Content = "IP Rate limit exceeded. Please try again later.";

            var rateLimitRule = new RateLimitRule
            {
                Endpoint = Endpoint,
                Period = $"{rateLimitConfig.PeriodInSeconds}s",
                Limit = rateLimitConfig.Limit,
                QuotaExceededResponse = quotaExceededResponse
            };

            // Process the request using the IP rate limiting processor
            var rateLimitCounter = await _ipRateLimitProcessor.ProcessRequestAsync(identity, rateLimitRule);

            // Check if the request has exceeded the rate limit
            return rateLimitCounter.Count <= rateLimitConfig.Limit;

        }
    }

}

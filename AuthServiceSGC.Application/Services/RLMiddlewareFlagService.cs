using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.Services
{
    public class RLMiddlewareFlagService
    {
        private volatile bool _flagValue;
        private readonly IDatabase _redisDb;
        private readonly ISubscriber _subscriber;
        private readonly string _redisChannel = "CustomRateLimitFlagChannel";

        public bool FlagValue => _flagValue;

        public RLMiddlewareFlagService(IDatabase redisDb, IConnectionMultiplexer redis)
        {

            _redisDb = redisDb;
            InitializeFlag();

            _subscriber = redis.GetSubscriber();

            // Subscribe to the Redis channel for flag updates
            _subscriber.Subscribe(_redisChannel, (channel, message) =>
            {
                if (bool.TryParse(message, out bool updatedFlag))
                {
                    _flagValue = updatedFlag;
                }
            });
        }

        private void InitializeFlag()
        {
            // Attempt to retrieve the flag value from Redis
            var cachedFlag = _redisDb.StringGet("RateLimitingMiddlewareFlagKey");

            // If the flag exists in Redis, use it; otherwise, default to false
            if (bool.TryParse(cachedFlag, out bool flag))
            {
                _flagValue = flag;
            }
            else
            {
                // Default value is false if Redis does not have the key
                _flagValue = false;

                // Optionally, set the default value in Redis if it's not there
                _redisDb.StringSet("RateLimitingMiddlewareFlagKey", _flagValue.ToString());
            }
        }

        public string UpdateFlag(UpdateFlagInputModel updateFlagInputModel)
        {
            //only for the first time setting key value in redis, remove porposekey post 1st usage
            //Or you set say todays date (exact string), and check if date is later than today don't change the string 
            if (updateFlagInputModel.porposeKey == 1)
            {
                _redisDb.StringSet("RateLimitFlagSecurityString", updateFlagInputModel.securityValue.ToString());
            }

            string status = string.Empty;
            if (updateFlagInputModel.securityValue == _redisDb.StringGet("RateLimitFlagSecurityString"))
            {
                _redisDb.StringSet("RateLimitingMiddlewareFlagKey", updateFlagInputModel.newFlagValue.ToString());

                // Publish the update to all subscribers
                _subscriber.Publish(_redisChannel, updateFlagInputModel.newFlagValue.ToString());
                return status;
            }
            else
            {
                status = "Given security value is wrong";
                return status;
            }
            //return status;
        }
    }

    public class CustomRateLimitingModel
    {
        public Dictionary<string, EndpointOrGroupReference> Endpoints { get; set; }
        public Dictionary<string, EndpointRateLimitConfig> Groups { get; set; }
    }
    public class EndpointOrGroupReference
    {
        public int? Limit { get; set; }
        public int? PeriodInSeconds { get; set; }
        public int? Type { get; set; }
        public string? Group { get; set; }
    }
    public class EndpointRateLimitConfig
    {
        public int Limit { get; set; }
        public int PeriodInSeconds { get; set; }
        public int Type { get; set; }
    }

    // 
    public class UpdateFlagInputModel
    {
        public bool newFlagValue { get; set; }
        public string securityValue { get; set; }
        public int? porposeKey { get; set; }
    }
}

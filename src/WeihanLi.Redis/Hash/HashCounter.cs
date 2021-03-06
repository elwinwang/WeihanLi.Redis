﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class HashCounterClient : BaseRedisClient, IHashCounterClient
    {
        private readonly string _realKey;

        public HashCounterClient(ILogger<HashCounterClient> logger, IRedisWrapper redisWrapper, string key) : this(logger, redisWrapper, key, 0)
        {
        }

        public HashCounterClient(ILogger<HashCounterClient> logger, IRedisWrapper redisWrapper, string key, long @base) : base(logger, redisWrapper)
        {
            _realKey = Wrapper.GetRealKey(key);
            Base = @base;
        }

        public long Base { get; }

        public long Count(RedisValue field, CommandFlags flags = CommandFlags.None)
        {
            return long.TryParse(Wrapper.Database.HashGet(_realKey, field, flags), out var count)
                ? count
                : Base;
        }

        public bool Reset(RedisValue field, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashSet(_realKey, field, Base, flags: flags);

        public Task<bool> ResetAsync(RedisValue field, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashSetAsync(_realKey, field, Base, flags: flags);

        public long Increase(RedisValue field, CommandFlags flags = CommandFlags.None) => Increase(field, 1, flags);

        public long Decrease(RedisValue field, CommandFlags flags = CommandFlags.None) => Decrease(field, 1, flags);

        public long Increase(RedisValue field, int step, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashIncrement(_realKey, field, step, flags);

        public long Decrease(RedisValue field, int step, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashDecrement(_realKey, field, step, flags);

        public Task<long> IncreaseAsync(RedisValue field, CommandFlags flags = CommandFlags.None) => IncreaseAsync(field, 1, flags);

        public Task<long> DecreaseAsync(RedisValue field, CommandFlags flags = CommandFlags.None) => DecreaseAsync(field, 1, flags);

        public Task<long> IncreaseAsync(RedisValue field, int step, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashIncrementAsync(_realKey, field, step, flags);

        public Task<long> DecreaseAsync(RedisValue field, int step, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashDecrementAsync(_realKey, field, step, flags);
    }
}

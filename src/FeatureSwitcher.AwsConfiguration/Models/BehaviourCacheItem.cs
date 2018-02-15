using System;
using FeatureSwitcher.AwsConfiguration.Behaviours;

namespace FeatureSwitcher.AwsConfiguration.Models
{
    internal class BehaviourCacheItem
    {
        public IBehaviour Behaviour { get; }
        public DateTime CacheTimeout { get; }

        public bool IsExpired => this.CacheTimeout < DateTime.UtcNow;

        public BehaviourCacheItem(IBehaviour behaviour, TimeSpan cacheTime)
        {
            Behaviour = behaviour;
            CacheTimeout = DateTime.UtcNow.Add(cacheTime);
        }

        public void ExtendCache()
        {
            CacheTimeout.Add(TimeSpan.FromMinutes(1));
        }
    }
}

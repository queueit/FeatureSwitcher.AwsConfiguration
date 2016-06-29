using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FeatureSwitcher.AwsConfiguration.Behaviours;
using FeatureSwitcher.AwsConfiguration.Models;

namespace FeatureSwitcher.AwsConfiguration
{
    internal class BehaviourProvider
    {
        private readonly string _apiEndpoint;
        private readonly IRestClient _restClient;
        private readonly TimeSpan _cacheTimeout;
        private readonly IBehaviourFactory _behaviourFactory;

        internal BehaviourProvider(
            string apiEndpoint, 
            IRestClient restClient, 
            TimeSpan cacheTimeout, 
            IBehaviourFactory behaviourFactory)
        {
            this._restClient = restClient;
            this._cacheTimeout = cacheTimeout;
            this._behaviourFactory = behaviourFactory ?? new DefaultBehaviourFactory();
            if (apiEndpoint == null)
                throw new ArgumentNullException(apiEndpoint);

            this._apiEndpoint = apiEndpoint.EndsWith("/") ? apiEndpoint : apiEndpoint + "/";
        }

        readonly ConcurrentDictionary<string, BehaviourCacheItem> _cache = new ConcurrentDictionary<string, BehaviourCacheItem>(); 

        public void Setup()
        {
            var task = SetupAsync();

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        public Task SetupAsync()
        {
            Type[] features = FindAllFeatures();

            List<Task> tasks = new List<Task>();

            foreach (var feature in features)
            {
                tasks.Add(LoadConfigFromService(feature.FullName));
            }

            return Task.WhenAll(tasks);
        }

        private Type[] FindAllFeatures()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domain => domain.GetTypes())
                .Where(type => !type.IsInterface && !type.IsAbstract && typeof(IFeature).IsAssignableFrom(type) )
                .ToArray();
        }

        private async Task LoadConfigFromService(string featureName)
        {
            SetFallbackBehaviour(featureName);

                var featureConfig = await GetConfigurationFromService(featureName).ConfigureAwait(false);

                if (FeatureConfigIsValid(featureConfig))
                    AddOrUpdateCache(featureName, featureConfig);
                else
                    await CreateConfigurationEntry(featureName).ConfigureAwait(false);
        }

        private Task<dynamic> GetConfigurationFromService(string featureName)
        {
            return this._restClient
                .GetAsync(GetFeatureEndpoint(featureName));
        }

        private string GetFeatureEndpoint(string featureName)
        {
            return string.Concat(
                this._apiEndpoint,
                "feature?FeatureName=",
                featureName);
        }

        private Task<dynamic> CreateConfigurationEntry(string featureName)
        {
            return this._restClient.PutAsync(GetFeatureEndpoint(featureName), null);
        }

        private void AddOrUpdateCache(string featureName, dynamic featureConfig)
        {
            var featureBehaviour = this._behaviourFactory.Create(featureConfig["type"].ToString());

            if (featureBehaviour != null)
            {
                featureBehaviour.SetConfiguration(featureConfig["value"]);

                _cache.AddOrUpdate(
                    featureName,
                    (key) => new BehaviourCacheItem(featureBehaviour, this._cacheTimeout),
                    (key, value) =>
                    {
                        this._behaviourFactory.Release(value.Behaviour);
                        return new BehaviourCacheItem(featureBehaviour, this._cacheTimeout);
                    });
            }
        }

        private static dynamic FeatureConfigIsValid(dynamic featureConfig)
        {
            return featureConfig != null && 
                   !string.IsNullOrEmpty(featureConfig["type"]) && 
                   featureConfig["type"] != "\"\"" &&
                   featureConfig["value"] != null;
        }

        private void SetFallbackBehaviour(string featureName)
        {
            _cache.AddOrUpdate(
                featureName,
                (name) => new BehaviourCacheItem(new BooleanBehaviour(false), this._cacheTimeout),
                (name, behaviour) => behaviour);
            return;
        }

        public IBehaviour GetBehaviour(Feature.Name name)
        {
            var cacheItem = this._cache[name.Value];
            if (cacheItem.IsExpired)
            {
                lock (cacheItem)    
                {
                    if (cacheItem.IsExpired)
                    {
                        cacheItem.ExtendCache();
                        FireAndForgetLoadConfigFromService(name);
                    }
                }
            }

            return cacheItem.Behaviour;

        }

        private void FireAndForgetLoadConfigFromService(Feature.Name name)
        {
            // async fire and forget
            Task.Run(() =>
            {
                this.LoadConfigFromService(name.Value);
            });
        }
    }
}

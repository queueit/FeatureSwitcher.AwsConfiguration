﻿using System;
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

        internal BehaviourProvider(string apiEndpoint, IRestClient restClient, TimeSpan cacheTimeout)
        {
            this._restClient = restClient;
            _cacheTimeout = cacheTimeout;
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

        public async Task SetupAsync()
        {
            Type[] features = FindAllFeatures();

            List<Task> tasks = new List<Task>();

            foreach (var feature in features)
            {
                tasks.Add(LoadConfigFromService(feature.FullName));
            }

            await Task.WhenAll(tasks);
        }

        private Type FindType(string fullName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domain => domain.GetTypes())
                .FirstOrDefault(type => type.FullName == fullName);
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

            try
            {
                var featureConfig = await GetConfigurationFromService(featureName);

                if (FeatureConfigIsValid(featureConfig))
                    AddOrUpdateCache(featureName, featureConfig);
                else
                    await CreateConfigurationEntry(featureName);
            }
            catch (ConfigurationRequestException ex)
            {
                throw ex;
            }
        }

        private async Task<dynamic> GetConfigurationFromService(string featureName)
        {
            var featureConfig = await this._restClient
                .GetAsync(GetFeatureEndpoint(featureName));
            return featureConfig;
        }

        private string GetFeatureEndpoint(string featureName)
        {
            return string.Concat(
                this._apiEndpoint,
                "feature?FeatureName=",
                featureName);
        }

        private async Task<dynamic> CreateConfigurationEntry(string featureName)
        {
            return await this._restClient.PutAsync(GetFeatureEndpoint(featureName), null);
        }

        private void AddOrUpdateCache(string featureName, dynamic featureConfig)
        {
            Type behaviourType = this.FindType(featureConfig["type"].ToString());

            if (behaviourType != null)
            {
                var featureBehaviour = Activator.CreateInstance(behaviourType) as IBehaviour;

                if (featureBehaviour != null)
                {
                    featureBehaviour.SetConfiguration(featureConfig["value"]);

                    _cache.AddOrUpdate(
                        featureName,
                        (key) => new BehaviourCacheItem(featureBehaviour, this._cacheTimeout),
                        (key, value) => new BehaviourCacheItem(featureBehaviour, this._cacheTimeout));
                }
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
                (name, behaviour) => new BehaviourCacheItem(new BooleanBehaviour(false), this._cacheTimeout));
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
                        this.LoadConfigFromService(name.Value); // async fire and forget
                    }
                }
            }

            return cacheItem.Behaviour;

        }
    }
}
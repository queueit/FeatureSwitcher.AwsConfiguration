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

        internal BehaviourProvider(string apiEndpoint, IRestClient restClient)
        {
            this._restClient = restClient;
            if (apiEndpoint == null)
                throw new ArgumentNullException(apiEndpoint);

            this._apiEndpoint = apiEndpoint.EndsWith("/") ? apiEndpoint : apiEndpoint + "/";
        }

        readonly ConcurrentDictionary<string, BehaviourCacheItem> cache = new ConcurrentDictionary<string, BehaviourCacheItem>(); 

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
                var featureConfig = await this._restClient
                    .GetAsync(string.Concat(
                        this._apiEndpoint,
                        "feature?FeatureName=",
                        featureName));

                if (!FeatureConfigIsValid(featureConfig))
                    return;

                Type behaviourType = this.FindType(featureConfig["Type"].ToString());

                if (behaviourType != null)
                {
                    var featureBehaviour = Activator.CreateInstance(behaviourType) as IBehaviour;

                    if (featureBehaviour != null)
                    {
                        featureBehaviour.SetConfiguration(featureConfig["Value"]);

                        cache.AddOrUpdate(
                            featureName,
                            (key) => new BehaviourCacheItem(featureBehaviour),
                            (key, value) => new BehaviourCacheItem(featureBehaviour));
                    }
                }
            }
            catch (ConfigurationRequestException ex)
            {
                throw ex;
            }
        }

        private static dynamic FeatureConfigIsValid(dynamic featureConfig)
        {
            return featureConfig != null && 
                   !string.IsNullOrEmpty(featureConfig["Type"]) && 
                   featureConfig["Type"] != "\"\"" &&
                   featureConfig["Value"] != null;
        }

        private void SetFallbackBehaviour(string featureName)
        {
            cache.AddOrUpdate(
                featureName,
                (name) => new BehaviourCacheItem(new BooleanBehaviour(false)),
                (name, behaviour) => new BehaviourCacheItem(new BooleanBehaviour(false)));
            return;
        }

        public IBehaviour GetBehaviour(Feature.Name name)
        {
            return this.cache[name.Value].Behaviour;
        }
    }
}

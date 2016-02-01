using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSwitcher.AwsConfiguration.Behaviours
{
    public class AwsConfig
    {
        private BehaviourProvider BehaviourProvider { get; }

        public bool? IsEnabled(Feature.Name name)
        {
            var behaviour = BehaviourProvider.GetBehaviour(name);
            return behaviour.Behaviour(name);
        }

        internal static async Task<AwsConfig> ConfigureAsync(
            string apiEndpoint, IRestClient restClient, TimeSpan cacheTimeout = default(TimeSpan))
        {
            AwsConfig config = new AwsConfig(apiEndpoint, restClient, cacheTimeout);
            await config.BehaviourProvider.SetupAsync();
            return config;
        }

        public static async Task<AwsConfig> ConfigureAsync(
            string apiEndpoint, TimeSpan cacheTimeout = default(TimeSpan))
        {
            AwsConfig config = new AwsConfig(apiEndpoint, cacheTimeout);
            await config.BehaviourProvider.SetupAsync();
            return config;
        }

        internal static AwsConfig Configure(
            string apiEndpoint, IRestClient restClient, TimeSpan cacheTimeout = default(TimeSpan))
        {
            AwsConfig config = new AwsConfig(apiEndpoint, restClient, cacheTimeout);
            config.BehaviourProvider.Setup();
            return config;

        }

        public static AwsConfig Configure(
            string apiEndpoint, TimeSpan cacheTimeout = default(TimeSpan))
        {
            AwsConfig config = new AwsConfig(apiEndpoint, cacheTimeout);
            config.BehaviourProvider.Setup();
            return config;

        }

        private AwsConfig(string apiEndpoint, TimeSpan cacheTimeout)
            : this(apiEndpoint, new RestClient(), cacheTimeout)
        {}

        private AwsConfig(string apiEndpoint, IRestClient restClient, TimeSpan cacheTimeout)
        {
            if (cacheTimeout == default(TimeSpan))
                cacheTimeout = TimeSpan.FromMinutes(5);

            this.BehaviourProvider = new BehaviourProvider(apiEndpoint, restClient, cacheTimeout);
        }
    }
}

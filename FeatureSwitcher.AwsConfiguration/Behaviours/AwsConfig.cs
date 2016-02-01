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
        internal BehaviourProvider BehaviourProvider { get; private set; }

        public bool? IsEnabled(Feature.Name name)
        {
            var behaviour = BehaviourProvider.GetBehaviour(name);
            return behaviour.Behaviour(name);
        }

        public static async Task<AwsConfig> ConfigureAsync(string apiEndpoint, IRestClient restClient)
        {
            AwsConfig config = new AwsConfig(apiEndpoint, restClient);
            await config.BehaviourProvider.SetupAsync();
            return config;
        }

        public static async Task<AwsConfig> ConfigureAsync(string apiEndpoint)
        {
            AwsConfig config = new AwsConfig(apiEndpoint);
            await config.BehaviourProvider.SetupAsync();
            return config;
        }

        internal static AwsConfig Configure(string apiEndpoint, IRestClient restClient)
        {
            AwsConfig config = new AwsConfig(apiEndpoint, restClient);
            config.BehaviourProvider.Setup();
            return config;

        }

        public static AwsConfig Configure(string apiEndpoint)
        {
            AwsConfig config = new AwsConfig(apiEndpoint);
            config.BehaviourProvider.Setup();
            return config;

        }

        public AwsConfig(string apiEndpoint)
            : this(apiEndpoint, new RestClient())
        {}

        private AwsConfig(string apiEndpoint, IRestClient restClient)
        {
            this.BehaviourProvider = new BehaviourProvider(apiEndpoint, restClient);
        }
    }
}

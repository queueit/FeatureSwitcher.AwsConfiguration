using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using FeatureSwitcher.AwsConfiguration.Behaviours;
using FeatureSwitcher.Configuration;
using Rhino.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace FeatureSwitcher.AwsConfiguration.Tests
{
    public class BehaviourProviderTest
    {
        private readonly ITestOutputHelper output;

        public BehaviourProviderTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        private object TestFeatureBooleanEnabled = new
        {
            FeatureName = typeof (TestFeature).FullName,
            Type = typeof (BooleanBehaviour).FullName,
            Value = new { BOOL = true }
        };

        private object TestFeatureBooleanDisabled = new
        {
            FeatureName = typeof(TestFeature).FullName,
            Type = typeof(BooleanBehaviour).FullName,
            Value = new { BOOL = false }
        };

        [Fact(Skip = "Integration test for debugging")]
        public void BehaviourProvider_Setup_Integration_Test()
        {
            var config = AwsConfig.Configure("https://id.execute-api.eu-west-1.amazonaws.com/test");

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature>.Is().Enabled);
        }

        [Fact]
        public void BehaviourProvider_Setup_NoConfigurationAvailable_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null)).IgnoreArguments()
                .Return(GeneratedGetResponse());

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature>.Is().Disabled);
        }

        [Fact]
        public void BehaviourProvider_Setup_ConfigurationAvailable_Bollean_Enabled_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null)).IgnoreArguments()
                .Return(GeneratedGetResponse(TestFeatureBooleanEnabled));

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature>.Is().Enabled);
        }

        [Fact]
        public void BehaviourProvider_Setup_ConfigurationAvailable_Bollean_Disabled_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null)).IgnoreArguments()
                .Return(GeneratedGetResponse(TestFeatureBooleanDisabled));

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature>.Is().Disabled);
        }

        [Fact]
        public void BehaviourProvider_Setup_ServiceError_Enabled_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null)).IgnoreArguments()
                .Throw(new ConfigurationRequestException(HttpStatusCode.InternalServerError, null, null));

            Exception ex = Assert.Throws<ConfigurationRequestException>(() =>
            {
                AwsConfig.Configure(
                    "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test",
                    restClient);
            });
        }

        [Fact]
        public async void BehaviourProvider_SetupAsync_ServiceError_Enabled_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null)).IgnoreArguments()
                .Throw(new ConfigurationRequestException(HttpStatusCode.InternalServerError, null, null));

            var config = AwsConfig.ConfigureAsync(
                "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test",
                restClient);

            var ex = await Assert.ThrowsAsync<ConfigurationRequestException>(async () =>
            {
                await config;
            });
        }


        private Task<dynamic> GeneratedGetResponse(object config = null)
        {
            if (config == null)
                config = new {FeatureName = "", Type = "", Value = ""};

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var response = serializer.Serialize(config);
            return Task.FromResult<dynamic>(serializer.Deserialize<dynamic>(response));
        }
    }
}

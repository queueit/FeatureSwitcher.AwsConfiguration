using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FeatureSwitcher.AwsConfiguration.Behaviours;
using FeatureSwitcher.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FeatureSwitcher.AwsConfiguration.Tests
{
    public class BehaviourProviderTest
    {
        private object TestFeatureBooleanEnabled = new
        {
            type = typeof (BooleanBehaviour).FullName,
            value = new { BOOL = true }
        };

        private object TestFeatureBooleanDisabled = new
        {
            type = typeof(BooleanBehaviour).FullName,
            value = new { BOOL = false }
        };

        [Fact(Skip = "Integration test")]
        public void BehaviourProvider_Setup_Integration_Test()
        {
            var config = AwsConfig.Configure("https://[id].execute-api.[region].amazonaws.com/test");

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Enabled);
        }

        [Fact(Skip = "Integration test")]
        public async void BehaviourProvider_SetupAsync_Integration_Test()
        {
            var configTask = AwsConfig.ConfigureAsync("https://[id].execute-api.[region].amazonaws.com/test");

            // Do stuff

            var config = await configTask;

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Enabled);
        }

        [Fact]
        public void BehaviourProvider_Setup_RestCall_Test()
        {
            IRestClient restClient = Substitute.For<IRestClient>();
            restClient.GetAsync(null)
                .ReturnsForAnyArgs(GeneratedGetResponse());

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            restClient.Received().GetAsync(
                "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test/feature?FeatureName=FeatureSwitcher.AwsConfiguration.Tests.TestFeature1");
        }

        [Fact]
        public void BehaviourProvider_Setup_NoConfigurationAvailable_Test()
        {
            IRestClient restClient = Substitute.For<IRestClient>();
            restClient.GetAsync(null)
                .ReturnsForAnyArgs(GeneratedGetResponse());

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Disabled);

            restClient.Received().PutAsync(
    "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test/feature?FeatureName=FeatureSwitcher.AwsConfiguration.Tests.TestFeature1"
    );

        }

        [Fact]
        public void BehaviourProvider_Setup_NoConfigurationAvailable_CreateNewConfiguration_Test()
        {
            IRestClient restClient = Substitute.For<IRestClient>();
            restClient.GetAsync(null)
                .ReturnsForAnyArgs(GeneratedGetResponse());

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            restClient.Received().PutAsync(
                "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test/feature?FeatureName=FeatureSwitcher.AwsConfiguration.Tests.TestFeature1"
                );
        }

        [Fact]
        public void BehaviourProvider_Setup_ConfigurationAvailable_Boolean_Enabled_Test()
        {
            IRestClient restClient = Substitute.For<IRestClient>();
            restClient.GetAsync(null)
                .ReturnsForAnyArgs(GeneratedGetResponse(TestFeatureBooleanEnabled));

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Enabled);
        }

        [Fact]
        public void BehaviourProvider_Setup_ConfigurationAvailable_Boolean_Disabled_Test()
        {
            IRestClient restClient = Substitute.For<IRestClient>();
            restClient.GetAsync(null)
                .ReturnsForAnyArgs(GeneratedGetResponse(TestFeatureBooleanDisabled));

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Disabled);
        }

        [Fact]
        public void BehaviourProvider_Setup_ServiceError_Enabled_Test()
        {
            IRestClient restClient = Substitute.For<IRestClient>();
            restClient.GetAsync(null)
                .ThrowsForAnyArgs(new ConfigurationRequestException(HttpStatusCode.InternalServerError, null, null));

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
            IRestClient restClient = Substitute.For<IRestClient>();
            restClient.GetAsync(null)
                .ThrowsForAnyArgs(new ConfigurationRequestException(HttpStatusCode.InternalServerError, null, null));

            var config = AwsConfig.ConfigureAsync(
                "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test",
                restClient);

            var ex = await Assert.ThrowsAsync<ConfigurationRequestException>(async () =>
            {
                await config;
            });
        }

        [Fact(Skip = "Integration")]
        public void BehaviourProvider_CacheTimeout_Test()
        {
            IRestClient restClient = Substitute.For<IRestClient>();
            restClient.GetAsync(null)
                .ReturnsForAnyArgs(GeneratedGetResponse(TestFeatureBooleanEnabled),
                GeneratedGetResponse(TestFeatureBooleanDisabled));

            var config = AwsConfig.Configure(
                "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", 
                restClient,
                TimeSpan.FromMilliseconds(100));

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Enabled);
            Assert.True(Feature<TestFeature1>.Is().Enabled);
            Assert.True(Feature<TestFeature1>.Is().Enabled);
            Assert.True(Feature<TestFeature1>.Is().Enabled);
            Assert.True(Feature<TestFeature1>.Is().Enabled);
            Assert.True(Feature<TestFeature1>.Is().Enabled);

            Thread.Sleep(101); //expire cache

            var x = Feature<TestFeature1>.Is().Enabled; //call once to make new request to service

            restClient.Received(2)
                .GetAsync(
                    "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test/feature?FeatureName=FeatureSwitcher.AwsConfiguration.Tests.TestFeature1");

            Assert.False(Feature<TestFeature1>.Is().Enabled);
        }

        [Fact]
        public void BehaviourProvider_Setup_CustomFactory_Test()
        {
            IRestClient restClient = Substitute.For<IRestClient>();
            restClient.GetAsync(null)
                .ReturnsForAnyArgs(GeneratedGetResponse(TestFeatureBooleanDisabled));
            IBehaviourFactory behaviourFactory = Substitute.For<IBehaviourFactory>();
            behaviourFactory.Create(null)
                .ReturnsForAnyArgs(x => throw new TestBehaviourFactoryException());

            Assert.Throws<TestBehaviourFactoryException>(() =>
            {
                var config = AwsConfig.Configure(
                    "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test",
                    restClient,
                    behaviourFactory: behaviourFactory);
            });
        }

        private Task<string> GeneratedGetResponse(object config = null)
        {
            if (config == null)
                config = new {type = "", value = ""};

            var response = JsonConvert.SerializeObject(config);
            return Task.FromResult(response);
        }
    }

    public class TestBehaviourFactoryException : Exception
    {
    }
}

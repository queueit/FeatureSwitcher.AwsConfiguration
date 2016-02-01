using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using FeatureSwitcher.AwsConfiguration.Behaviours;
using FeatureSwitcher.Configuration;
using Rhino.Mocks;
using Xunit;

namespace FeatureSwitcher.AwsConfiguration.Tests
{
    public class BehaviourProviderTest
    {
        private object TestFeatureBooleanEnabled = new
        {
            FeatureName = typeof (TestFeature1).FullName,
            Type = typeof (BooleanBehaviour).FullName,
            Value = new { BOOL = true }
        };

        private object TestFeatureBooleanDisabled = new
        {
            FeatureName = typeof(TestFeature1).FullName,
            Type = typeof(BooleanBehaviour).FullName,
            Value = new { BOOL = false }
        };

        [Fact(Skip = "Integration test")]
        public void BehaviourProvider_Setup_Integration_Test()
        {
            var config = AwsConfig.Configure("https://b82jcihcsc.execute-api.eu-west-1.amazonaws.com/test");

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Enabled);
        }

        [Fact(Skip = "Integration test")]
        public async void BehaviourProvider_SetupAsync_Integration_Test()
        {
            var configTask = AwsConfig.ConfigureAsync("https://b82jcihcsc.execute-api.eu-west-1.amazonaws.com/test");

            // Do stuff

            var config = await configTask;

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Enabled);
        }

        [Fact]
        public void BehaviourProvider_Setup_RestCall_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null)).IgnoreArguments()
                .Return(GeneratedGetResponse());
            restClient.Stub(client => client.PutAsync(null, null)).IgnoreArguments()
                .Return(Task.FromResult<dynamic>(null));

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            restClient.AssertWasCalled(client => client.GetAsync(
                "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test/feature?FeatureName=FeatureSwitcher.AwsConfiguration.Tests.TestFeature1"));
        }

        [Fact]
        public void BehaviourProvider_Setup_NoConfigurationAvailable_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null)).IgnoreArguments()
                .Return(GeneratedGetResponse());
            restClient.Stub(client => client.PutAsync(null, null)).IgnoreArguments()
                .Return(Task.FromResult<dynamic>(null));

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Disabled);

            restClient.AssertWasCalled(client => client.PutAsync(
    "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test/feature?FeatureName=FeatureSwitcher.AwsConfiguration.Tests.TestFeature1",
    null));

        }

        [Fact]
        public void BehaviourProvider_Setup_NoConfigurationAvailable_CreateNewConfiguration_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null)).IgnoreArguments()
                .Return(GeneratedGetResponse());
            restClient.Stub(client => client.PutAsync(null, null)).IgnoreArguments()
                .Return(Task.FromResult<dynamic>(null));

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            restClient.AssertWasCalled(client => client.PutAsync(
                "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test/feature?FeatureName=FeatureSwitcher.AwsConfiguration.Tests.TestFeature1",
                null));
        }

        [Fact]
        public void BehaviourProvider_Setup_ConfigurationAvailable_Boolean_Enabled_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null)).IgnoreArguments()
                .Return(GeneratedGetResponse(TestFeatureBooleanEnabled));

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Enabled);
        }

        [Fact]
        public void BehaviourProvider_Setup_ConfigurationAvailable_Boolean_Disabled_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null)).IgnoreArguments()
                .Return(GeneratedGetResponse(TestFeatureBooleanDisabled));

            var config = AwsConfig.Configure("https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test", restClient);

            Features.Are.ConfiguredBy.Custom(config.IsEnabled);

            Assert.True(Feature<TestFeature1>.Is().Disabled);
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

        [Fact]
        public void BehaviourProvider_CacheTimeout_Test()
        {
            IRestClient restClient = MockRepository.GenerateMock<IRestClient>();
            restClient.Stub(client => client.GetAsync(null))
                .IgnoreArguments()
                .Return(GeneratedGetResponse(TestFeatureBooleanEnabled))
                .Repeat.Once();
            restClient.Stub(client => client.GetAsync(null))
                .IgnoreArguments()
                .Return(GeneratedGetResponse(TestFeatureBooleanDisabled));

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

            restClient.AssertWasCalled(
                client => client.GetAsync(
                    "https://j3453jfdkh43.execute-api.eu-west-1.amazonaws.com/test/feature?FeatureName=FeatureSwitcher.AwsConfiguration.Tests.TestFeature1"),
                options => options.Repeat.Twice());

            Assert.False(Feature<TestFeature1>.Is().Enabled);
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

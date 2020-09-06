using FeatureSwitcher.AwsConfiguration.Behaviours;
using Xunit;

namespace FeatureSwitcher.AwsConfiguration.Tests.Behaviours
{
    public class BooleanBehaviourTest
    {
        private string BooleanBehaviourTemplate = @"{""type"":""FeatureSwitcher.AwsConfiguration.Behaviours.InCustomerListBehaviour"",""value"":INPUT}";

        [Fact]
        public void BooleanBehaviour_NoLoadedConfiguration_False_Test()
        {
            BooleanBehaviour boolean = new BooleanBehaviour();

            var enabled = boolean.Behaviour(new Feature.Name(typeof (TestFeature1), typeof (TestFeature1).FullName));

            Assert.False(enabled);
        }

        [Fact]
        public void BooleanBehaviour_SetConfiguration_False_Test()
        {
            var data = ConstructFeatureConfig.Execute(BooleanBehaviourTemplate.Replace("INPUT", "{\"BOOL\": false}"));

            BooleanBehaviour boolean = new BooleanBehaviour();
            boolean.SetConfiguration(data.GetValue());
            var enabled = boolean.Behaviour(new Feature.Name(typeof(TestFeature1), typeof(TestFeature1).FullName));

            Assert.False(enabled);
        }

        [Fact]
        public void BooleanBehaviour_SetConfiguration_True_Test()
        {
            var data = ConstructFeatureConfig.Execute(BooleanBehaviourTemplate.Replace("INPUT", "{\"BOOL\": true}"));

            BooleanBehaviour boolean = new BooleanBehaviour();
            boolean.SetConfiguration(data.GetValue());
            var enabled = boolean.Behaviour(new Feature.Name(typeof(TestFeature1), typeof(TestFeature1).FullName));

            Assert.True(enabled);
        }


    }
}

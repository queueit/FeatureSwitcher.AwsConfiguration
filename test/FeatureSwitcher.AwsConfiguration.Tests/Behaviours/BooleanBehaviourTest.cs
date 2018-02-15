using System.Collections.Generic;
using FeatureSwitcher.AwsConfiguration.Behaviours;
using Xunit;

namespace FeatureSwitcher.AwsConfiguration.Tests.Behaviours
{
    public class BooleanBehaviourTest
    {
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
            BooleanBehaviour boolean = new BooleanBehaviour();
            boolean.SetConfiguration(new Dictionary<string, bool> { { "BOOL", false } });
            var enabled = boolean.Behaviour(new Feature.Name(typeof(TestFeature1), typeof(TestFeature1).FullName));

            Assert.False(enabled);
        }

        [Fact]
        public void BooleanBehaviour_SetConfiguration_True_Test()
        {
            BooleanBehaviour boolean = new BooleanBehaviour();
            boolean.SetConfiguration(new Dictionary<string, bool> { { "BOOL", true }});
            var enabled = boolean.Behaviour(new Feature.Name(typeof(TestFeature1), typeof(TestFeature1).FullName));

            Assert.True(enabled);
        }


    }
}

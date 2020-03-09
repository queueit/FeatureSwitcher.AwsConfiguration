using FeatureSwitcher.AwsConfiguration.Behaviours;
using Xunit;

namespace FeatureSwitcher.AwsConfiguration.Tests.Behaviours
{
    public class IntegerBehaviourTest
    {
        [Fact]
        public void IntegerBehaviour_Deserialization_SameValue_Test()
        {
            var data = ConstructFeatureConfig.Execute("{\"N\": \"45\"}");

            var inList = new TestIntegerBehaviour(45);
            inList.SetConfiguration(data);

            var enabled = inList.Behaviour(new Feature.Name(typeof (TestFeature1), typeof (TestFeature1).FullName));

            Assert.True(enabled);
        }

        [Fact]
        public void IntegerBehaviour_Deserialization_OtherValue_Test()
        {
            var data = ConstructFeatureConfig.Execute("{\"N\": \"37\"}");

            var inList = new TestIntegerBehaviour(45);
            inList.SetConfiguration(data);

            var enabled = inList.Behaviour(new Feature.Name(typeof(TestFeature1), typeof(TestFeature1).FullName));

            Assert.False(enabled);
        }

        [Fact]
        public void IntegerBehaviour_Deserialization_InvalidValue_Test()
        {
            var data = ConstructFeatureConfig.Execute("{\"S\": \"Invalid\"}");

            var inList = new TestIntegerBehaviour(45);
            inList.SetConfiguration(data);

            var enabled = inList.Behaviour(new Feature.Name(typeof(TestFeature1), typeof(TestFeature1).FullName));

            Assert.False(enabled);
        }

    }

    public class TestIntegerBehaviour : IntegerBehaviour
    {
        private readonly int _value;

        public TestIntegerBehaviour(int value)
        {
            _value = value;
        }

        protected override bool IsEnabled()
        {
            return ConfiguredValue == this._value;
        }
    }
}

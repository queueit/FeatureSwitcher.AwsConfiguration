using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using FeatureSwitcher.AwsConfiguration.Behaviours;
using Xunit;

namespace FeatureSwitcher.AwsConfiguration.Tests.Behaviours
{
    public class InStringListBehaviourTest
    {
        [Fact]
        public void InStringListBehaviour_Deserialization_InList_Test()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var data = serializer.Deserialize<dynamic>("{\"L\": [{\"S\": \"queueitprod\"}]}");

            TestInStringListBehaviour inList = new TestInStringListBehaviour("queueitprod");
            inList.SetConfiguration(data);

            var enabled = inList.Behaviour(new Feature.Name(typeof (TestFeature1), typeof (TestFeature1).FullName));

            Assert.True(enabled);
        }

        [Fact]
        public void InStringListBehaviour_Deserialization_SameValueTwice_Test()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var data = serializer.Deserialize<dynamic>("{\"L\": [{\"S\": \"queueitprod\"},{\"S\": \"queueitprod\"}]}");

            TestInStringListBehaviour inList = new TestInStringListBehaviour("queueitprod");
            inList.SetConfiguration(data);

            var enabled = inList.Behaviour(new Feature.Name(typeof(TestFeature1), typeof(TestFeature1).FullName));

            Assert.True(enabled);
        }


        [Fact]
        public void InStringListBehaviour_Deserialization_NotInList_Test()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var data = serializer.Deserialize<dynamic>("{\"L\": [{\"S\": \"queueitprod\"}]}");

            TestInStringListBehaviour inList = new TestInStringListBehaviour("mala");
            inList.SetConfiguration(data);

            var enabled = inList.Behaviour(new Feature.Name(typeof(TestFeature1), typeof(TestFeature1).FullName));

            Assert.False(enabled);
        }

    }

    public class TestInStringListBehaviour : InStringListBehaviour
    {
        private readonly string _value;

        public TestInStringListBehaviour(string value)
        {
            _value = value;
        }

        protected override bool IsInList()
        {
            return this.IsInList(this._value);
        }
    }
}

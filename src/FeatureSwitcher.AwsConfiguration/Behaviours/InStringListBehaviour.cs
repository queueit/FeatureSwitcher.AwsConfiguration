using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeatureSwitcher.AwsConfiguration.Behaviours
{
    public abstract class InStringListBehaviour : IBehaviour
    {
        private HashSet<string> _list = new HashSet<string>();

        public bool? Behaviour(Feature.Name name)
        {
            return this.IsInList();
        }

        public void SetConfiguration(dynamic configValue)
        {
            if (configValue == null)
                return;

            try
            {
                var value = (InStringListBehaviourValueDto)JsonSerializer.Deserialize<InStringListBehaviourValueDto>(configValue);
                if (value.DynamoDBStringList == null)
                {
                    return; // fallback to false;
                }
                _list = new HashSet<string>(value.DynamoDBStringList.Select(x => x.Value));
            }
            catch (JsonException)
            {
                // fallback to false;
            }
        }

        protected bool IsInList(string value)
        {
            if (value == null)
                return false;

            return this._list.Contains(value);
        }

        protected abstract bool IsInList();
    }

    public class InStringListBehaviourValueDto
    {
        [JsonPropertyName("L")]
        public DynamoDBStringList[] DynamoDBStringList { get; set; }
    }

    public class DynamoDBStringList
    {
        [JsonPropertyName("S")]
        public string Value { get; set; }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeatureSwitcher.AwsConfiguration.Behaviours
{
    public class BooleanBehaviour : IBehaviour
    {
        private bool _boolValue = false;

        public BooleanBehaviour()
        { }

        public BooleanBehaviour(bool value)
        {
            this._boolValue = value;
        }

        public void SetConfiguration(dynamic configValue)
        {
            if (configValue == null)
                return;

            try
            {
                var value = (BooleanBehaviourValueDto)JsonSerializer.Deserialize<BooleanBehaviourValueDto>(configValue);
                this._boolValue = value.Value;
            }
            catch (JsonException)
            {
                // fallback to false;
            }
        }

        public bool? Behaviour(Feature.Name name)
        {
            return this._boolValue;
        }
    }

    public class BooleanBehaviourValueDto
    {
        [JsonPropertyName("BOOL")]
        public bool Value { get; set; }
    }

}

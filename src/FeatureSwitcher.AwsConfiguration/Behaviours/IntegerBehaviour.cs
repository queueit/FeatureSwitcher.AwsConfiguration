using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeatureSwitcher.AwsConfiguration.Behaviours
{
    public abstract class IntegerBehaviour : IBehaviour
    {
        protected int ConfiguredValue = 0;

        public IntegerBehaviour()
        { }

        public IntegerBehaviour(int value)
        {
            ConfiguredValue = value;
        }

        public void SetConfiguration(dynamic configValue)
        {
            if (configValue == null)
                return;

            try
            {
                var value = (IntegerBehaviourValueDto)JsonSerializer.Deserialize<IntegerBehaviourValueDto>(configValue);
                if (value.Value == null)
                {
                    return; // fallback to 0;
                }

                ConfiguredValue = int.Parse(value.Value);
            }
            catch (JsonException)
            {
                // fallback to 0;
            }
        }

        public bool? Behaviour(Feature.Name name)
        {
            return IsEnabled();
        }

        protected abstract bool IsEnabled();
    }

    public class IntegerBehaviourValueDto
    {
        [JsonPropertyName("N")]
        public string Value { get; set; }
    }
}
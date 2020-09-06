using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeatureSwitcher.AwsConfiguration
{
    public static class ConstructFeatureConfig
    {
        public static BaseFeatureConfigDto Execute(string response)
        {
            return JsonSerializer.Deserialize<BaseFeatureConfigDto>(response);
        }
    }

    public class BaseFeatureConfigDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("value")]
        public object Value { get; set; }

        public string GetValue()
        {
            return Value?.ToString();
        }
    }
}

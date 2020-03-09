using Newtonsoft.Json.Linq;

namespace FeatureSwitcher.AwsConfiguration
{
    public static class ConstructFeatureConfig
    {
        public static dynamic Execute(string response)
        {
            return JObject.Parse(response);
        }
    }
}

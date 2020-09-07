namespace FeatureSwitcher.AwsConfiguration.Behaviours
{
    public interface IBehaviour
    {
        bool? Behaviour(Feature.Name name);
        void SetConfiguration(string configValue);
    }
}

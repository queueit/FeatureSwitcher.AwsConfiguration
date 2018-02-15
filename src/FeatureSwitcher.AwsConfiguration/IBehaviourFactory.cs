using FeatureSwitcher.AwsConfiguration.Behaviours;

namespace FeatureSwitcher.AwsConfiguration
{
    public interface IBehaviourFactory
    {
        IBehaviour Create(string behaviourType);
        void Release(IBehaviour behaviour);
    }
}

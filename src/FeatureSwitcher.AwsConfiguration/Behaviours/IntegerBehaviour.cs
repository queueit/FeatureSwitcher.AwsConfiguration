using Microsoft.CSharp.RuntimeBinder;

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
                ConfiguredValue = configValue["N"];
            }
            catch (RuntimeBinderException)
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
}
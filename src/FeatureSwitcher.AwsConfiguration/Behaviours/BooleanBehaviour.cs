using Microsoft.CSharp.RuntimeBinder;

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
                this._boolValue = configValue["BOOL"];
            }
            catch (RuntimeBinderException)
            {
                // fallback to false;
            }
        }

        public bool? Behaviour(Feature.Name name)
        {
            return this._boolValue;
        }
    }
}

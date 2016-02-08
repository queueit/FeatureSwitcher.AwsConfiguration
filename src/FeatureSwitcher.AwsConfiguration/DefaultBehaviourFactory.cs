using System;
using System.Linq;
using FeatureSwitcher.AwsConfiguration.Behaviours;

namespace FeatureSwitcher.AwsConfiguration
{
    internal class DefaultBehaviourFactory : IBehaviourFactory
    {
        public IBehaviour Create(string behaviourType)
        {
            if (string.IsNullOrEmpty(behaviourType))
                return null;

            Type type = this.FindType(behaviourType);

            if (type == null)
                return null;
            
            return Activator.CreateInstance(type) as IBehaviour;
        }

        public void Release(IBehaviour behaviour)
        {
        }

        private Type FindType(string fullName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domain => domain.GetTypes())
                .FirstOrDefault(type => type.FullName == fullName);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureSwitcher.AwsConfiguration.Behaviours;

namespace FeatureSwitcher.AwsConfiguration.Models
{
    internal class BehaviourCacheItem
    {
        public IBehaviour Behaviour { get; }

        public BehaviourCacheItem(IBehaviour behaviour)
        {
            Behaviour = behaviour;
        }
    }
}

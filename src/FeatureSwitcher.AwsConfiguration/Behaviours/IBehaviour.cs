using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSwitcher.AwsConfiguration.Behaviours
{
    public interface IBehaviour
    {
        bool? Behaviour(Feature.Name name);
        void SetConfiguration(dynamic configValue);
    }
}

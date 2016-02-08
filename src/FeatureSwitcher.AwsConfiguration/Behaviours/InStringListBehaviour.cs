using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;

namespace FeatureSwitcher.AwsConfiguration.Behaviours
{
    public abstract class InStringListBehaviour : IBehaviour
    {
        private Dictionary<string, object> _list = new Dictionary<string, object>();

        public bool? Behaviour(Feature.Name name)
        {
            return this.IsInList();
        }

        public void SetConfiguration(dynamic configValue)
        {
            if (configValue == null)
                return;

            try
            {
                this._list = this.DeserializeList(configValue);
            }
            catch (RuntimeBinderException ex)
            {
                // fallback to false;
            }
        }

        protected bool IsInList(string value)
        {
            return this._list.ContainsKey(value);
        }

        protected abstract bool IsInList();

        private Dictionary<string, object> DeserializeList(dynamic configValue)
        { 
            Dictionary<string, object> itemsInList = new Dictionary<string, object>();
            var list = configValue["L"];
            for (int i = 0; i < list.Length; i++)
            {
                itemsInList.Add(list[i]["S"] as string, null);
            }

            return itemsInList;
        }
    }
}

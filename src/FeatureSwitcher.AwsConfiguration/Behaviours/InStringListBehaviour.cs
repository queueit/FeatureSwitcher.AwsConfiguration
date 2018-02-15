﻿using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;

namespace FeatureSwitcher.AwsConfiguration.Behaviours
{
    public abstract class InStringListBehaviour : IBehaviour
    {
        private HashSet<string> _list = new HashSet<string>();

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
            if (value == null)
                return false;

            return this._list.Contains(value);
        }

        protected abstract bool IsInList();

        private HashSet<string> DeserializeList(dynamic configValue)
        { 
            HashSet<string> itemsInList = new HashSet<string>();
            var list = configValue["L"];
            for (int i = 0; i < list.Count; i++)
            {
                string value = list[i]["S"].ToString();
                if (!itemsInList.Contains(value))
                    itemsInList.Add(value);
            }

            return itemsInList;
        }
    }
}

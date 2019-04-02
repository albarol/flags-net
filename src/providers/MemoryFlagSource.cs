using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace FlagsNet.Providers
{
    public class MemoryFlagSource : IFlagSource
    {
        private readonly IDictionary<string, FlagParameter> source;

        public MemoryFlagSource()
        {
            this.source = new Dictionary<string, FlagParameter>();
        }

        public void Activate(string key)
        {
            if (!source.ContainsKey(key))
                return;
            source[key].Activated = true;
        }

        public void Add(string key, FlagParameter parameter)
        {
            if (!source.ContainsKey(key))
                source.Add(key, parameter);
            else
                source[key] = parameter;
        }

        public void Deactivate(string key)
        {
            if (!source.ContainsKey(key))
                return;
            source[key].Activated = false;
        }

        public bool Switch(string key)
        {
            if (!source.ContainsKey(key)) return false;
            var flag = source[key];
            return flag.Activated && string.IsNullOrEmpty(flag.Value);
        }

        public bool Switch<T>(string key, Predicate<T> predicate)
        {
            if (!source.ContainsKey(key)) return false;
            var flag = source[key];

            if (!flag.Activated) return false;
            var deserialized = JsonConvert.DeserializeObject<IList<T>>(flag.Value);
            foreach (var item in deserialized)
                if (predicate(item))
                    return true;
            return false;
        }
    }
}
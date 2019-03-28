using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace FlagsNet.Providers
{
    public class MemoryFlagSource : IFlagSource
    {
        private readonly IDictionary<string, KeyValuePair<FlagStatus, string>> source;

        public MemoryFlagSource()
        {
            this.source = new Dictionary<string, KeyValuePair<FlagStatus, string>>();
        }

        public void Activate(string key)
        {
            if (!source.ContainsKey(key))
                return;
            source[key] = new KeyValuePair<FlagStatus, string>(FlagStatus.Activated, source[key].Value);
        }

        public void Add(string key, FlagParameter parameter, FlagStatus status)
        {
            source[key] = new KeyValuePair<FlagStatus, string>(status, parameter.Value);
        }

        public void Deactivate(string key)
        {
            source[key] = new KeyValuePair<FlagStatus, string>(FlagStatus.Deactivated, source[key].Value);
        }

        public bool Switch(string key)
        {
            if (!source.ContainsKey(key)) return false;
            return source[key].Key != FlagStatus.Deactivated;
        }

        public bool Switch<T>(string key, Predicate<T> predicate)
        {
            if (!source.ContainsKey(key)) return false;
            var flag = source[key];

            if (flag.Key == FlagStatus.Deactivated) return false;
            var deserialized = JsonConvert.DeserializeObject<IList<T>>(flag.Value);
            foreach (var item in deserialized)
                if (predicate(item))
                    return true;
            return false;
        }
    }
}
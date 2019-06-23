using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlagsNet.Providers
{
    public class MemoryFlagSource : IFlagSource
    {
        private readonly IDictionary<string, FlagParameter> source;

        public MemoryFlagSource()
        {
            this.source = new Dictionary<string, FlagParameter>();
        }

        public IEnumerable<String> GetFlags()
        {
            return source.Keys;
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

        public bool Switch(string key, string jsonPath)
        {
            if (!source.ContainsKey(key)) return false;
            var flag = source[key];

            if (!flag.Activated) return false;
            JArray o = JArray.Parse(flag.Value);
            return o.SelectTokens(jsonPath).Count() > 0;
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
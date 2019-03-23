using System;
using System.Collections.Generic;

namespace FlagsNet.Providers
{
    public class MemoryFlagSource : IFlagSource
    {
        private readonly IDictionary<string, string> source;

        public MemoryFlagSource(IDictionary<string, string> source) {
            this.source = source;
        }

        public bool Switch(string key)
        {
            if (!source.ContainsKey(key)) return false;
            return Convert.ToBoolean(source[key]);
        }

        public bool Switch(string key, params string[] conditions)
        {
            if (!source.ContainsKey(key)) return false;
            HashSet<string> hashFlag = new HashSet<string>(source[key].Split('|'));
            HashSet<string> hashCondition = new HashSet<string>(conditions);
            hashCondition.IntersectWith(hashFlag);
            return hashCondition.Count > 0;
        }
    }
}
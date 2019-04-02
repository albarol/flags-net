using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace FlagsNet
{
    public class Manager
    {
        private readonly IFlagSource source;

        public Manager(IFlagSource source)
        {
            this.source = source;
        }

        public bool Active(string flag)
        {
            return source.Switch(flag);
        }

        public bool Active<T>(string flag, Predicate<T> predicate)
        {
            return source.Switch(flag, predicate);
        }

        public void Activate(string flag)
        {
            source.Activate(flag);
        }

        public void Add(string flag, bool activated)
        {
            source.Add(flag, FlagParameter.CreateEmpty(activated));
        }

        public void Add<T>(string flag, T parameter, bool activated)
        {
            if (IsList(parameter))
                source.Add(flag, FlagParameter.CreateFromObject(parameter, activated));
            else
                source.Add(flag, FlagParameter.CreateFromObject(new List<T>{parameter}, activated));
        }

        public void Load(string content)
        {
            var flags = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, object>>>(content);
            foreach (var key in flags.Keys) {
                var value = flags[key]["Conditions"];
                var activated = Convert.ToBoolean(flags[key]["Activated"]);

                if (value == null) {
                    source.Add(key, FlagParameter.CreateEmpty(activated));
                } else {
                    source.Add(key, FlagParameter.CreateFromObject(value, activated));
                }
            }
        }

        private bool IsList(object obj)
        {
            if (obj == null) return false;

            var type = obj.GetType();
            return obj is IList && type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }
    }
}

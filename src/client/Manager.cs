using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FlagsNet.Providers;
using Newtonsoft.Json;

namespace FlagsNet
{
    public class Manager
    {
        private readonly IFlagSource primarySource;
        private readonly IFlagSource readOnlySource;

        private CircuitBreaker circuitBreaker;

        public Manager(IFlagSource primarySource)
        {
            this.primarySource = primarySource;
            this.readOnlySource = new MemoryFlagSource();
            this.circuitBreaker = new CircuitBreaker();
        }

        private IFlagSource Source
        {
            get
            {
                if (circuitBreaker.Status == CircuitStatus.Open)
                    return readOnlySource;
                return primarySource;
            }
        }

        public IEnumerable<string> GetFlags()
        {
            try
            {
                return Source.GetFlags();
            }
            catch (FlagSourceException) {
                circuitBreaker.SetFail();
                return readOnlySource.GetFlags();
            }
        }

        public bool Active(string flag)
        {
            try
            {
                return Source.Switch(flag);
            }
            catch(FlagSourceException)
            {
                circuitBreaker.SetFail();
                return readOnlySource.Switch(flag);
            }
        }

        public bool Active<T>(string flag, Predicate<T> predicate)
        {
            try
            {
                return Source.Switch(flag, predicate);
            }
            catch(FlagSourceException)
            {
                circuitBreaker.SetFail();
                return readOnlySource.Switch(flag, predicate);
            }
        }

        public void Activate(string flag)
        {
            try
            {
                primarySource.Activate(flag);
            }
            catch(FlagSourceException e)
            {
                circuitBreaker.SetFail();
                throw e;
            }
        }

        public void Add(string flag, bool activated)
        {
            try
            {
                primarySource.Add(flag, FlagParameter.CreateEmpty(activated));
            }
            catch(FlagSourceException e)
            {
                circuitBreaker.SetFail();
                throw e;
            }
        }

        public void Add<T>(string flag, T parameter, bool activated)
        {
            try
            {
                if (IsList(parameter))
                    primarySource.Add(flag, FlagParameter.CreateFromObject(parameter, activated));
                else
                    primarySource.Add(flag, FlagParameter.CreateFromObject(new List<T>{parameter}, activated));
            }
            catch(FlagSourceException e)
            {
                circuitBreaker.SetFail();
                throw e;
            }
        }

        public void Load(string content)
        {
            var flags = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, object>>>(content);
            foreach (var key in flags.Keys) {
                var value = flags[key]["Conditions"];
                var activated = Convert.ToBoolean(flags[key]["Activated"]);

                if (value == null) {
                    primarySource.Add(key, FlagParameter.CreateEmpty(activated));
                    readOnlySource.Add(key, FlagParameter.CreateEmpty(activated));
                } else {
                    primarySource.Add(key, FlagParameter.CreateFromObject(value, activated));
                    readOnlySource.Add(key, FlagParameter.CreateFromObject(value, activated));
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

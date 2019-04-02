using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace FlagsNet.Providers
{
    public class RedisFlagSource : IFlagSource
    {
        private readonly string ACTIVATED_FLAG = "Activated";
        private readonly string VALUE_FLAG = "Value";

        ConnectionMultiplexer connection;
        IDatabase db;

        public RedisFlagSource(string hostname) {
            connection = ConnectionMultiplexer.Connect(hostname);
            db = connection.GetDatabase();
        }

        public void Activate(string key)
        {
            if (!db.HashExists(key, ACTIVATED_FLAG))
                return;
            db.HashSet(key, new HashEntry[] { new HashEntry(ACTIVATED_FLAG, Boolean.TrueString)});
        }

        public void Add(string key, FlagParameter parameter)
        {
            db.HashSet(key, new HashEntry[] {
                new HashEntry(ACTIVATED_FLAG, parameter.Activated ? Boolean.TrueString : Boolean.FalseString),
                new HashEntry(VALUE_FLAG, parameter.Value),
            });
        }

        public void Deactivate(string key)
        {
            if (!db.HashExists(key, ACTIVATED_FLAG))
                return;
            db.HashSet(key, new HashEntry[] { new HashEntry(ACTIVATED_FLAG, Boolean.FalseString)});
        }

        public bool Switch(string key)
        {
            if (!HasFeature(key)) return false;
            var activated = Convert.ToBoolean(db.HashGet(key, ACTIVATED_FLAG));
            var value = db.HashGet(key, VALUE_FLAG);
            return activated && string.IsNullOrEmpty(value);
        }

        public bool Switch<T>(string key, Predicate<T> predicate)
        {
            if (!HasFeature(key)) return false;

            var activated = Convert.ToBoolean(db.HashGet(key, ACTIVATED_FLAG));
            if (!activated) return false;

            var value = db.HashGet(key, VALUE_FLAG);
            var deserialized = JsonConvert.DeserializeObject<IList<T>>(value);
            foreach (var item in deserialized)
                if (predicate(item))
                    return true;
            return false;
        }

        private bool HasFeature(string key)
        {
            return db.HashExists(key, ACTIVATED_FLAG) && db.HashExists(key, VALUE_FLAG);
        }
    }
}
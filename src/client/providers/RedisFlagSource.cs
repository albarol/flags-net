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
        private readonly string VALUE_FLAG = "Conditions";
        private readonly string KEYS_FLAG = "Keys";

        string hostname;

        public RedisFlagSource(string hostname)
        {
            this.hostname = hostname;
        }

        private IDatabase Database
        {
            get
            {
                try
                {
                    var connection = ConnectionMultiplexer.Connect(hostname);
                    return connection.GetDatabase();
                }
                catch
                {
                    throw new FlagSourceException("Client was not able to connect.");
                }
            }
        }

        public IEnumerable<string> GetFlags()
        {
            return Database.ListRange(KEYS_FLAG).Select(v => v.ToString());
        }

        public void Activate(string key)
        {
            var db = Database;
            if (!db.HashExists(key, ACTIVATED_FLAG))
                return;
            db.HashSet(key, new HashEntry[] { new HashEntry(ACTIVATED_FLAG, Boolean.TrueString)});
        }

        public void Add(string key, FlagParameter parameter)
        {
            var db = Database;
            db.HashSet(key, new HashEntry[] {
                new HashEntry(ACTIVATED_FLAG, parameter.Activated ? Boolean.TrueString : Boolean.FalseString),
                new HashEntry(VALUE_FLAG, parameter.Value),
            });
            db.ListRightPush(KEYS_FLAG, key);
        }

        public void Deactivate(string key)
        {
            var db = Database;
            if (!db.HashExists(key, ACTIVATED_FLAG))
                return;
            db.HashSet(key, new HashEntry[] { new HashEntry(ACTIVATED_FLAG, Boolean.FalseString)});
        }

        public bool Switch(string key)
        {
            var db = Database;
            if (!HasFeature(key)) return false;
            var activated = Convert.ToBoolean(db.HashGet(key, ACTIVATED_FLAG));
            var value = db.HashGet(key, VALUE_FLAG);
            return activated && string.IsNullOrEmpty(value);
        }

        public bool Switch<T>(string key, Predicate<T> predicate)
        {
            var db = Database;
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
            var db = Database;
            return db.HashExists(key, ACTIVATED_FLAG) && db.HashExists(key, VALUE_FLAG);
        }
    }
}
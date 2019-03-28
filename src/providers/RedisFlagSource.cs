using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StackExchange.Redis;

namespace FlagsNet.Providers
{
    public class RedisFlagSource : IFlagSource
    {
        ConnectionMultiplexer connection;
        IDatabase db;

        public RedisFlagSource(string hostname) {
            connection = ConnectionMultiplexer.Connect(hostname);
            db = connection.GetDatabase();
        }

        public void Activate(string key)
        {
            throw new NotImplementedException();
        }

        public void Add(string key, FlagParameter parameter, FlagStatus status)
        {
            throw new NotImplementedException();
        }

        public void Deactivate(string key)
        {
            throw new NotImplementedException();
        }

        public bool Switch(string flag)
        {
            var value = db.StringGet(flag);
            return Convert.ToBoolean(value);
        }

        public bool Switch(string flag, params string[] conditions)
        {
            var members = db.SetMembers(flag);
            if (members.Length == 0) return false;

            var hashMembers = new HashSet<string>(members.Select(m => m.ToString()));
            var hashConditions = new HashSet<string>(conditions);
            hashConditions.IntersectWith(hashMembers);
            return hashConditions.Count > 0;
        }
        public bool Switch<T>(string key, Predicate<T> expression)
        //public bool Switch<T>(string key, Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
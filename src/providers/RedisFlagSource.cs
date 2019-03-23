using System.Collections.Generic;
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

        public bool Switch(string key)
        {
            return true;
        }

        public bool Switch(string key, params string[] conditions)
        {
            throw new System.NotImplementedException();
        }
    }
}
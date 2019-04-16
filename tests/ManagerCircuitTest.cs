
using System.Collections.Generic;
using NUnit.Framework;

using FlagsNet.Providers;
using System;

namespace FlagsNet.Tests
{
    public class MemoryCircuitTest : ManagerTest
    {
        protected override Manager CreateManager()
        {
            return new Manager(new WriteOnlyFlagSource());
        }
    }

    class WriteOnlyFlagSource : MemoryFlagSource
    {
        public bool Switch(string key) { throw new FlagSourceException(); }
        public bool Switch<T>(string key, Predicate<T> expression) { throw new FlagSourceException(); }
        public IEnumerable<string> GetFlags() { throw new FlagSourceException(); }
    }
}
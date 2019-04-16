using System.Collections.Generic;
using NUnit.Framework;

using FlagsNet.Providers;
using System;

namespace FlagsNet.Tests
{
    //[Ignore("Just run it if you have a instance of redis running")]
    public class RedisManagerTest : ManagerTest
    {
        protected override Manager CreateManager()
        {
            return new Manager(new RedisFlagSource("localhost:6379"));
        }
    }
}
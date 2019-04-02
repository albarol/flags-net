using System.Collections.Generic;
using NUnit.Framework;

using FlagsNet.Providers;
using System;

namespace FlagsNet.Tests
{
    public class MemoryManagerTest : ManagerTest
    {
        protected override Manager CreateManager()
        {
            return new Manager(new MemoryFlagSource());
        }
    }
}
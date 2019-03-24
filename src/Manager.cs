using System;
using System.Collections.Generic;

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

        public bool Active(string flag, params string[] conditions)
        {
            return source.Switch(flag, conditions);
        }
    }
}

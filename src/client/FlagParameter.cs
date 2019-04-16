using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlagsNet
{
    public sealed class FlagParameter
    {
        public string Value { get; private set; }
        public bool Activated { get; internal set; }

        private FlagParameter() { }

        internal static FlagParameter CreateFromObject<T>(T value, bool activated = false)
        {
            var serialized = JsonConvert.SerializeObject(value);
            return new FlagParameter { Value = serialized, Activated = activated };
        }

        internal static FlagParameter CreateEmpty(bool activated = false)
        {
            return new FlagParameter { Value = string.Empty, Activated = activated };
        }

        public static implicit operator FlagParameter(string value)
        {
            return new FlagParameter { Value = value, Activated = false };
        }

        public static implicit operator string(FlagParameter value)
        {
            return value.Value;
        }
    }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlagsNet
{
    public sealed class FlagParameter
    {
        public string Value { get; private set; }

        private FlagParameter() { }

        internal static FlagParameter CreateFromObject<T>(T value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            return new FlagParameter { Value = serialized };
        }

        internal static FlagParameter Empty
        {
            get
            {
                return new FlagParameter { Value = string.Empty };
            }
        }

        public static implicit operator FlagParameter(string value)
        {
            return new FlagParameter { Value = value };
        }

        public static implicit operator string(FlagParameter value)
        {
            return value.Value;
        }
    }
}
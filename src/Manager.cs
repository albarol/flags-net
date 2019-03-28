using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public bool Active<T>(string flag, Predicate<T> predicate)
        {
            return source.Switch(flag, predicate);
        }

        public void Activate(string flag)
        {
            source.Activate(flag);
        }

        public void Add(string flag, FlagStatus status)
        {
            source.Add(flag, FlagParameter.Empty, status);
        }

        public void Add<T>(string flag, T parameter, FlagStatus status)
        {
            if (IsList(parameter))
                source.Add(flag, FlagParameter.CreateFromObject(parameter), status);
            else
                source.Add(flag, FlagParameter.CreateFromObject(new List<T>{parameter}), status);
        }

        private bool IsList(object obj)
        {
            if (obj == null) return false;

            var type = obj.GetType();
            return obj is IList && type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }
    }
}

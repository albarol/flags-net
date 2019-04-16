using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlagsNet
{
    public interface IFlagSource
    {
        void Activate(string key);
        void Deactivate(string key);
        void Add(string key, FlagParameter parameter);

        bool Switch(string key);
        bool Switch<T>(string key, Predicate<T> expression);

        IEnumerable<string> GetFlags();
    }
}
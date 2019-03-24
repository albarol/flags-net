using System.Collections.Generic;

namespace FlagsNet
{
    public interface IFlagSource
    {
        bool Switch(string key);
        bool Switch(string key, params string[] conditions);
    }
}
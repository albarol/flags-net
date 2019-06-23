using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlagsNet.Filters
{
    public class PathBuilder
    {
        public static string Parse(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var pathBase = "$[?({0})]";
            var fragments = parameters.Select(kv => GetPathFragment(kv.Key, kv.Value));
            var jsonPath = string.Format(pathBase, string.Join(" && ", fragments.Select(f => f.Fragment)));

            var inFragments = fragments.Where(f => f.Operator == "in").ToList();
            if (inFragments.Count > 1) {
                throw new PathFilterException("Operator 'in' is only supported once.");
            }
            else if (inFragments.Count == 1)
            {
                var inFragment = inFragments[0];
                jsonPath += string.Format(".{0}[?(@ == '{1}')]", inFragment.Name, inFragment.Value);
            }
            return jsonPath;
        }

        private static PathFragment GetPathFragment(string key, string value)
        {
            var pathOperator = key.Split(new[] {"__"}, StringSplitOptions.None);
            var name = pathOperator[0];
            var op = (pathOperator.Length > 1 ? pathOperator[1] : "eq");
            return new PathFragment {Name = name, Operator = op, Value = value};
        }
    }

}
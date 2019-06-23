namespace FlagsNet.Filters {

    internal struct PathFragment
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }

        public string Fragment
        {
            get
            {
                switch (Operator) {
                    case "in":
                        return string.Format("@.{0}", Name);
                    case "ne":
                        return string.Format("@.{0} != '{1}'", Name, Value);
                    case "gt":
                        return string.Format("@.{0} > '{1}'", Name, Value);
                    case "gte":
                        return string.Format("@.{0} >= '{1}'", Name, Value);
                    case "lt":
                        return string.Format("@.{0} < '{1}'", Name, Value);
                    case "lte":
                        return string.Format("@.{0} <= '{1}'", Name, Value);
                    default:
                        return string.Format("@.{0} == '{1}'", Name, Value);
                }
            }
        }
    }
}
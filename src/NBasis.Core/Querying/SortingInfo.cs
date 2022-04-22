namespace NBasis.Querying
{
    public class SortingInfo
    {
        public IEnumerable<SortProperty> SortBy { get; }

        public SortingInfo(string name, SortDirection direction = SortDirection.Ascending)
        {
            SortBy = new SortProperty(name, direction).Yield().ToArray();
        }

        public SortingInfo(IEnumerable<SortProperty> properties)
        {
            SortBy = properties.ToArray();
        }

        public override string ToString()
        {
            return $"{string.Join(",", SortBy)}";
        }
    }

    public class SortProperty
    {
        public SortProperty(string name, SortDirection direction = SortDirection.Ascending)
        {
            Name = name;
            Direction = direction;
        }

        public string Name { get;  }

        public SortDirection Direction { get; }

        public override string ToString()
        {
            return Direction == SortDirection.Descending ? "-" + Name : Name;
        }
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }
}

namespace NBasis.Querying
{
    public class PagingInfo
    {
        public PagingInfo(int take, int skip = 0)
        {
            Take = take;
            Skip = skip;
        }

        public PagingInfo(int take, string cursor = null)
        {
            Take = take;
            Cursor = cursor;
        }

        /// <summary>
        /// Max number of results to return
        /// </summary>
        public int Take { get; }

        /// <summary>
        /// Number of results to skip
        /// </summary>
        public int Skip { get; }

        /// <summary>
        /// A marker from which to continue
        /// </summary>
        public string Cursor { get; }

        public PagingInfo Previous()
        {
            if (!string.IsNullOrWhiteSpace(Cursor)) return null;

            if (Skip < Take)
                return null;
            return new PagingInfo(Take, Math.Max(Skip - Take, 0));
        }

        public PagingInfo First()
        {
            return new PagingInfo(Take, 0);
        }

        public PagingInfo Next(int previousTake = -1)
        {
            if (!string.IsNullOrWhiteSpace(Cursor)) return null;

            if ((previousTake >= 0) && (previousTake < Take))
                return null;
            return new PagingInfo(Take, Skip + Take);
        }
    }
}

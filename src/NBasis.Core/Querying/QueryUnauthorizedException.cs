using System;

namespace NBasis.Querying
{
    public class QueryUnauthorizedException : Exception
    {
        public QueryUnauthorizedException() : base() { }

        public QueryUnauthorizedException(string message) : base(message) { }
    }
}

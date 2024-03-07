using NBasis.Handling;
using System.ComponentModel.DataAnnotations;

namespace NBasis.Querying
{
    public abstract class QueryHandler<TQuery, TResult> : IHandleQueries<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private IHandlingContext<TQuery, TResult> _context;

        Task<TResult> IHandler<TQuery, TResult>.HandleAsync(IHandlingContext<TQuery, TResult> handlingContext)
        {
            _context = handlingContext;

            Validate(handlingContext.Input);

            return Handle(handlingContext.Input);
        }

        public IDictionary<string, object> Headers
        {
            get
            {
                if ((_context != null) && (_context.Headers != null))
                    return _context.Headers;
                return null;
            }
        }

        public virtual void Validate(TQuery query)
        {
            var validationContext = new ValidationContext(query, null, null);
            Validator.ValidateObject(query, validationContext, true);
        }

        public abstract Task<TResult> Handle(TQuery query);
    }
}

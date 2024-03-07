using NBasis.Handling;
using System.ComponentModel.DataAnnotations;

namespace NBasis.Commanding
{
    public abstract class CommandHandler<TCommand, TResult> : IHandleCommands<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        private IHandlingContext<TCommand, TResult> _context;

        Task<TResult> IHandler<TCommand, TResult>.HandleAsync(IHandlingContext<TCommand, TResult> handlingContext)
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

        public virtual void Validate(TCommand command)
        {
            var validationContext = new ValidationContext(command, null, null);
            Validator.ValidateObject(command, validationContext, true);
        }

        public abstract Task<TResult> Handle(TCommand command);
    }
}

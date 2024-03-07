using NBasis.Handling;

namespace NBasis.Commanding
{
    /// <summary>
    /// Marker interface for all command handlers
    /// </summary>
    public interface IHandleCommands
    {
    }

    public interface IHandleCommands<TCommand, TResult> : IHandler<TCommand, TResult>, IHandleCommands where TCommand : ICommand<TResult>
    {   
    }
}

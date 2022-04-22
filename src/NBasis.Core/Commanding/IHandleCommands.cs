namespace NBasis.Commanding
{
    /// <summary>
    /// Marker interface for all command handlers
    /// </summary>
    public interface IHandleCommands
    {
    }

    public interface IHandleCommands<TCommand> : IHandleCommands where TCommand : ICommand
    {
        Task HandleAsync(ICommandHandlingContext<TCommand> handlingContext);
    }
}

namespace NBasis.Commanding
{
    public interface ICommander
    {
        /// <summary>
        /// Ask a command to execute and return a result
        /// </summary>
        Task<TResult> AskAsync<TCommand, TResult>(Envelope<TCommand> command) where TCommand : ICommand<TResult>;
    }
}

namespace NBasis.Commanding
{
    public interface ICommander
    {
        /// <summary>
        /// Send a command and return a result
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        Task<TResult> AskAsync<TResult>(Envelope<ICommand<TResult>> command);
    }
}

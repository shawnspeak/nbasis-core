namespace NBasis.Commanding
{
    /// <summary>
    /// Marker interface for all commands
    /// </summary>
    public interface ICommand
    {
    }

    /// <summary>
    /// Command that returns a result
    /// </summary>
    public interface ICommand<TResult> : ICommand
    {
    }
}

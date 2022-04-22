namespace NBasis.Querying
{
    public interface IQueryDispatcherFactory
    {
        IQueryDispatcher GetQueryDispatcher(IServiceProvider serviceProvider);
    }
}

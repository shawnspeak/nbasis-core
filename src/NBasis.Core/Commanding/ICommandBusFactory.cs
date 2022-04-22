namespace NBasis.Commanding
{
    public interface ICommandBusFactory
    {
        ICommandBus GetCommandBus(IServiceProvider serviceProvider, CommandOptions options = null);
    }
}

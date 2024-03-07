namespace NBasis.Commanding
{
    public interface ICommanderFactory
    {
        ICommander GetCommander(IServiceProvider serviceProvider, CommandOptions options = null);
    }
}

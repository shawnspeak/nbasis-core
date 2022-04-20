using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBasis.Commanding
{
    public interface ICommandBus
    {
        /// <summary>
        /// Send command(s) to the bus
        /// </summary>
        Task SendAsync(IEnumerable<Envelope<ICommand>> commands);

        /// <summary>
        /// Send a command and wait around for a result
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        Task<TResult> AskAsync<TResult>(Envelope<ICommand> command);
    }
}

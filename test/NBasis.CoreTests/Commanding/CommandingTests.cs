using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NBasis.Commanding;
using NBasis.Types;

namespace NBasis.CoreTests.Types
{
    public class TestCommand : ICommand<string>
    {
        public string Data { get; set; }
    }

    public class TestCommandHandler : CommandHandler<TestCommand, string>
    {
        public override Task<string> Handle(TestCommand command)
        {
            return Task.FromResult(command.Data);
        }
    }

    public class CommandingTests
    {
        private IServiceProvider RegisterCommander()
        {
            var collection = new ServiceCollection();
            collection.AddAssembliesToFinder(Assembly.GetExecutingAssembly())
                      .AddLocalCommanding();
            return collection.BuildServiceProvider();
        }

        [Fact(DisplayName = "Commander is registered")]
        public void CommanderIsRegistered()
        {
            var provider = RegisterCommander();
            var commander = provider.GetRequiredService<ICommander>();
            Assert.NotNull(commander);
        }

        [Fact(DisplayName = "Command is handled")]
        public async Task CommandHandlerTest()
        {
            var provider = RegisterCommander();
            var commander = provider.GetRequiredService<ICommander>();

            var result = await commander.AskAsync(new TestCommand { Data = "Hello" });
            Assert.Equal("Hello", result);
        }
    }
}
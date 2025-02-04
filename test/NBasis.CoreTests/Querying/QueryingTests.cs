using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NBasis.Querying;
using NBasis.Types;

namespace NBasis.CoreTests.Querying
{
    public class TestQuery : IQuery<string>
    {
        public string Data { get; set; }
    }

    public class TestQueryHandler : QueryHandler<TestQuery, string>
    {
        public override Task<string> Handle(TestQuery query)
        {
            return Task.FromResult(query.Data);
        }
    }

    public class QueryingTests
    {
        private IServiceProvider RegisterDispatcher()
        {
            var collection = new ServiceCollection();
            collection.AddAssembliesToFinder(Assembly.GetExecutingAssembly())
                      .AddLocalQueryDispatcher();
            return collection.BuildServiceProvider();
        }

        [Fact(DisplayName = "Dispatcher is registered")]
        public void DispatcherIsRegistered()
        {
            var provider = RegisterDispatcher();
            var dispatcher = provider.GetRequiredService<IQueryDispatcher>();
            Assert.NotNull(dispatcher);
        }

        [Fact(DisplayName = "Query is handled")]
        public async Task QueryHandlerTest()
        {
            var provider = RegisterDispatcher();
            var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

            var result = await dispatcher.DispatchAsync(new TestQuery { Data = "Hello" });
            Assert.Equal("Hello", result);
        }
    }
}
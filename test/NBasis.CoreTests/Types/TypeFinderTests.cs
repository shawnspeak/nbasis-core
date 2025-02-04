using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NBasis.Types;

namespace NBasis.CoreTests.Types
{
    public interface ITestInterface
    {
    }

    public class TestClass : ITestInterface
    {
    }

    public class TypeFinderTestsTest
    {
        [Fact]
        public void AssemblyTypeFinderTest()
        {
            var collection = new ServiceCollection();
            collection.AddAssembliesToFinder(Assembly.GetExecutingAssembly());
            var provider = collection.BuildServiceProvider();

            var typeFinder = provider.GetRequiredService<ITypeFinder>();
            Assert.NotNull(typeFinder);

            var types = typeFinder.GetInterfaceImplementations<ITestInterface>();
            Assert.Single(types);
            Assert.Contains(types, t => t == typeof(TestClass));
        }
    }
}
using NBasis.Identification;

namespace NBasis.CoreTests.Indentification
{
    public class KSuidTests
    {

        [Fact(DisplayName = "KSuid equality tests")]
        public void KSuid_equality_tests()
        {
            var k1 = KSuid.Parse("1vUoeoX6r67DcNfQHq9oTRuBXvF");
            var k2 = KSuid.Parse("1vUoeoX6r67DcNfQHq9oTRuBXvF");

            Assert.Equal(k1, k2);
            Assert.True(k1 == k2);
            Assert.False(k1 > k2);
            Assert.False(k1 < k2);

            Assert.True(k1 == "1vUoeoX6r67DcNfQHq9oTRuBXvF");
            Assert.Equal("1vUoeoX6r67DcNfQHq9oTRuBXvF", k1);
        }

        [Fact(DisplayName = "KSuid creation tests")]
        public void KSuid_creation_tests()
        {
            var k1 = KSuid.NewKSuid();
            var k2 = KSuid.Parse(k1.ToString());
            Assert.Equal(k1, k2);

            var k3 = new KSuid(k2.Payload, k2.Timestamp);
            Assert.Equal(k2, k3);
        }

        [Fact(DisplayName = "KSuid creation is ordered")]
        public async Task KSuid_creation_is_ordered()
        {
            var k1 = KSuid.NewKSuid();

            await Task.Delay(1000);

            var k2 = KSuid.NewKSuid();
            Assert.True(k2 > k1);
        }
    }
}

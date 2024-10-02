using FluentAssertions;
using GChan.Trackers;
using System.Collections.Concurrent;
using Xunit;

namespace GChan.UnitTest
{
    /// <summary>
    /// Tests asserting that <see cref="ConcurrentDictionary{TK, TV}"/> uses our <see cref="Asset.GetHashCode"/> implementation to insert and retrieve values.
    /// </summary>
    public class ConcurrentDictionaryHashCodeKeyTests
    {
        readonly static Thread thread = new Thread_4Chan("https://boards.4chan.org/hr/thread/123");

        // Two different image link instances, with the same data.
        readonly static Asset asset1 = new Asset(123, "http://test.com", "test123", 1, thread);
        readonly static Asset asset2 = new Asset(123, "http://test.com", "test123", 1, thread);

        [Fact]
        public void TestAddAndRetrieveWithIndexOperator()
        {
            var dict = new ConcurrentDictionary<Asset, int>();

            // Store the value using the first imagelink instance as the key.
            dict[asset1] = 1;

            // Try getting the value using the 2nd instance.
            dict[asset2].Should().Be(1);
        }

        [Fact]
        public void TestAddAndRetrieveWithTryMethods()
        {
            var dict = new ConcurrentDictionary<Asset, int>();

            // Store the value using the first imagelink instance as the key.
            dict.TryAdd(asset1, 1);

            // Try getting the value using the 2nd instance.
            dict.TryGetValue(asset2, out var result).Should().BeTrue();
            result.Should().Be(1);
        }
    }
}

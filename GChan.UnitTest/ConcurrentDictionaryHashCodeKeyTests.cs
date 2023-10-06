using FluentAssertions;
using GChan.Trackers;
using System.Collections.Concurrent;
using Xunit;

namespace GChan.UnitTest
{
    /// <summary>
    /// Tests asserting that <see cref="ConcurrentDictionary{TK, TV}"/> uses our <see cref="ImageLink.GetHashCode"/> implementation to insert and retrieve values.
    /// </summary>
    public class ConcurrentDictionaryHashCodeKeyTests
    {
        readonly static Thread thread = new Thread_4Chan("https://boards.4chan.org/hr/thread/123");

        // Two different image link instances, with the same data.
        readonly static ImageLink il1 = new ImageLink(123, "http://test.com", "test123", 1, thread);
        readonly static ImageLink il2 = new ImageLink(123, "http://test.com", "test123", 1, thread);

        [Fact]
        public void TestAddAndRetrieveWithIndexOperator()
        {
            var dict = new ConcurrentDictionary<ImageLink, int>();

            // Store the value using the first imagelink instance as the key.
            dict[il1] = 1;

            // Try getting the value using the 2nd instance.
            dict[il2].Should().Be(1);
        }

        [Fact]
        public void TestAddAndRetrieveWithTryMethods()
        {
            var dict = new ConcurrentDictionary<ImageLink, int>();

            // Store the value using the first imagelink instance as the key.
            dict.TryAdd(il1, 1);

            // Try getting the value using the 2nd instance.
            dict.TryGetValue(il2, out var result).Should().BeTrue();
            result.Should().Be(1);
        }
    }
}

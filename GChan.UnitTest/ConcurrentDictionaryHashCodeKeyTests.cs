using FluentAssertions;
using GChan.Models;
using GChan.Models.Trackers;
using GChan.Models.Trackers.Sites;
using System.Collections.Concurrent;
using Xunit;

namespace GChan.UnitTest
{
    /// <summary>
    /// Tests asserting that <see cref="ConcurrentDictionary{TK, TV}"/> uses our <see cref="Upload.GetHashCode"/> implementation to insert and retrieve values.
    /// </summary>
    public class ConcurrentDictionaryHashCodeKeyTests
    {
        readonly static Thread thread = new Thread_4Chan("https://boards.4chan.org/hr/thread/123");

        // Two different image link instances, with the same data.
        readonly static Upload asset1 = new Upload(123, "http://test.com", "test123", 1, thread);
        readonly static Upload asset2 = new Upload(123, "http://test.com", "test123", 1, thread);

        [Fact]
        public void TestAddAndRetrieveWithIndexOperator()
        {
            var dict = new ConcurrentDictionary<Upload, int>();

            // Store the value using the first imagelink instance as the key.
            dict[asset1] = 1;

            // Try getting the value using the 2nd instance.
            dict[asset2].Should().Be(1);
        }

        [Fact]
        public void TestAddAndRetrieveWithTryMethods()
        {
            var dict = new ConcurrentDictionary<Upload, int>();

            // Store the value using the first imagelink instance as the key.
            dict.TryAdd(asset1, 1);

            // Try getting the value using the 2nd instance.
            dict.TryGetValue(asset2, out var result).Should().BeTrue();
            result.Should().Be(1);
        }
    }
}

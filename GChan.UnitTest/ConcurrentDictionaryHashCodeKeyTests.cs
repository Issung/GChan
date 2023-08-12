using FluentAssertions;
using System.Collections.Concurrent;
using Xunit;

namespace GChan.UnitTest
{
    /// <summary>
    /// Tests asserting that <see cref="ConcurrentDictionary{TK, TV}"/> uses our <see cref="ImageLink.GetHashCode"/> implementation to insert and retrieve values.
    /// </summary>
    public class ConcurrentDictionaryHashCodeKeyTests
    {
        // Two different image link instances, with the same data.
        readonly ImageLink il1 = new ImageLink(123, "http://test.com", "test123");
        readonly ImageLink il2 = new ImageLink(123, "http://test.com", "test123");

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

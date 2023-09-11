using NUnit.Framework;

namespace Common.FluentAssertionsExtensions;

public static class EnumerableExtensions
{
    public static void ShouldAllBeEquivalent<T>(this IEnumerable<T> enumerable, T expected)
    {
        foreach (var element in enumerable)
        {
            if (element!.Equals(expected))
            {
                continue;
            }

            Assert.Fail();
        }
    }
}
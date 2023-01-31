namespace Common.Extensions;

public static class EnumerableExtensions
{
    public static T SelectRandom<T>(this T[] array)
    {
        return array[random.Next(array.Length)];
    }

    private static Random random = new();
}